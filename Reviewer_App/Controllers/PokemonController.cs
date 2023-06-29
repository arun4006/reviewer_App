using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Reviewer_App.Dtos;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PokemonController: Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        private readonly IReviewRepository _reviewRepository;

        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository
            ,IMapper mapper)
        {
           _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
           _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200,Type=typeof(IEnumerable<Pokemon>))]
        public IActionResult GetAllPokemons()
        {
            var data = _pokemonRepository.GetPokemons();
            var pokemons=_mapper.Map<List<PokemonDto>>(data);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);

        }

        [HttpGet("{Pokemonid}")]
        [ProducesResponseType(200, Type=typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int Pokemonid)
        {
            if (!_pokemonRepository.PokemonExist(Pokemonid))
                return NotFound();
            var data=_pokemonRepository.GetPokemonById(Pokemonid);
            var pokemon = _mapper.Map<PokemonDto>(data);

            if (!ModelState.IsValid)
              return BadRequest(ModelState);    
            return Ok(pokemon);
        }

        [HttpGet("{Pokemonid}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int Pokemonid)
        {
            if (!_pokemonRepository.PokemonExist(Pokemonid))
                return NotFound();
            
            var rating = _pokemonRepository.GetPokemonRating(Pokemonid);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);

            var pokemons = _pokemonRepository.GetPokemons()
                .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);


            if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId,
            [FromQuery] int ownerId, [FromQuery] int catId,
            [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExist(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExist(pokeId))
            {
                return NotFound();
            }

            var reviewsToDelete = _reviewRepository.GetReviewsByPokemon(pokeId);
            var pokemonToDelete = _pokemonRepository.GetPokemonById(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong when deleting reviews");
            }

            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return NoContent();
        }

    }
}
