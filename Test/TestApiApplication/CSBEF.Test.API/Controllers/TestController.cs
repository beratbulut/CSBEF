using Microsoft.AspNetCore.Mvc;

namespace CSBEF.Test.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("hello");
        }
    }
}