using CountryExplorer.Models;
using CountryExplorer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryExplorer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountrySummary>>> GetAllCountries()
        {
            try
            {
                var countries = await _countryService.GetAllCountriesAsync();
                return Ok(countries);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
            }
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<CountryDetail>> GetCountryByName(string name)
        {
            try
            {
                var country = await _countryService.GetCountryByNameAsync(name);
                if (country == null)
                {
                    return NotFound();
                }

                return Ok(country);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
            }
        }
    }
}
