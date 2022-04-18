using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Services;
using Structures.ViewModels;

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
            return Ok(new ApiResponse<CovidDataResponse>(result,
                result.FirstOrDefault()?.TotalCount,
                request.PageNumber, request.PageSize));
        }
    }
}