using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Reviewer_App.Dtos;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CountryController: Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryRepository countryRepository,IMapper mapper,
            ILogger<CountryController> logger)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into HomeController");
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetAllCountries()
        {
            var data = _countryRepository.GetCountries();
            var countries = _mapper.Map<List<CountryDto>>(data);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _logger.LogDebug("This is a debug message");
            _logger.LogInformation("This is an info message");
            _logger.LogWarning("This is a warning message ");
            _logger.LogError(new Exception(), "This is an error message");
            return Ok(countries);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int id)
        {
            if (!_countryRepository.CountryExist(id))
                return NotFound();
            var data = _countryRepository.GetCountry(id);
            var country = _mapper.Map<CountryDto>(data);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(country);
        }

        [HttpGet("country/{owner_id}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByOwner(int owner_id)
        {          
            var data = _countryRepository.GetCountryByOwner(owner_id);
            var country = _mapper.Map<CountryDto>(data);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("Owners/{id}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnersByCountry(int id)
        {
            var owners = _countryRepository.GetOwnersFromACountry(id);
           
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if (countryCreate == null)
                return BadRequest(ModelState);

            var country = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryCreate);

            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int countryId, [FromBody] CountryDto updatedCountry)
        {
            if (updatedCountry == null)
                return BadRequest(ModelState);

            if (countryId != updatedCountry.Id)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExist(countryId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var countryMap = _mapper.Map<Country>(updatedCountry);

            if (!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return NoContent();
        }

    }
}
