using Microsoft.AspNetCore.Mvc;
using Services;
using Structures.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace CovidDataWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidDataController : ControllerBase
    {
        private readonly ICovidDataService _covidDataService;

        public CovidDataController(ICovidDataService covidDataService)
        {
            _covidDataService = covidDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CovidDataRequest request)
        {
            var result = await _covidDataService.SearchAsync(request);

            if (result.Records.Any())
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpGet("daily")]
        public async Task<IActionResult> Daily([FromQuery] CovidDataRequest request)
        {
            var result = await _covidDataService.GetDailyBreakDownDataAsync(request);

            if (result.Records.Any())
            {
                return Ok(result);
            }

            return NotFound(result);
        }
    }
}