using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CovidDataWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Ok");
        }
    }
}