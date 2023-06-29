using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Reviewer_App.Dtos;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class OwnerController: Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;

        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository,
            IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetAllOwners()
        {
            var data = _ownerRepository.GetOwners();
            var owners = _mapper.Map<List<OwnerDto>>(data);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owners);

        }

        [Authorize(Roles = "Lead")]
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int id=1)
        {
            if (!_ownerRepository.IsOwnerExist(id))
                return NotFound();
            var data = _ownerRepository.GetOwner(id);
            var owner = _mapper.Map<OwnerDto>(data);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);
        }

        [HttpGet("owner/{pokemon_id}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnersByPokemon(int pokemon_id)
        {
            var data = _ownerRepository.GetOwnerofAPokemon(pokemon_id);
            var owner = _mapper.Map<List<OwnerDto>>(data);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("pokemon/{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int id)
        {
            if (!_ownerRepository.IsOwnerExist(id))
                return NotFound();
            var data = _ownerRepository.GetPokemonByOwner(id);
            var pokemons=_mapper.Map<List<PokemonDto>>(data);
           
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId,[FromBody] OwnerDto ownerCreate)
        {
            if (ownerCreate == null)
                return BadRequest(ModelState);

            var owners = _ownerRepository.GetOwners()
                .Where(c => c.Name.Trim().ToUpper() == ownerCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (owners != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(ownerCreate);
            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }


        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            if (updatedOwner == null)
                return BadRequest(ModelState);

            if (ownerId != updatedOwner.Id)
                return BadRequest(ModelState);

            if (!_ownerRepository.IsOwnerExist(ownerId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var ownerMap = _mapper.Map<Owner>(updatedOwner);

            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerRepository.IsOwnerExist(ownerId))
            {
                return NotFound();
            }

            var ownerToDelete = _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepository.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return NoContent();
        }
    }
}
