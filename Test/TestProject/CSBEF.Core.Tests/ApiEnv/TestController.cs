using System;
using CSBEF.Concretes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CSBEF.Core.Tests.ApiEnv
{
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IActionContextAccessor httpContextAccessor;

        public TestController(
            IActionContextAccessor httpContextAccessor
        )
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("test/CheckHttpContextAccessorInjection")]
        public IActionResult CheckHttpContextAccessorInjection()
        {
            try
            {
                return Ok(this.httpContextAccessor.ActionContext.HttpContext.Request.Path);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet("test/CountLoadedModule")]
        public IActionResult CountLoadedModule()
        {
            try
            {
                return Ok(GlobalConfiguration.Modules.Count);
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}