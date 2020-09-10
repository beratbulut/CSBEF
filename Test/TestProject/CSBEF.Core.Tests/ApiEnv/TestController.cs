using System;
using System.Linq;
using CSBEF.Concretes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CSBEF.Core.Tests.ApiEnv
{
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IActionContextAccessor httpContextAccessor;
        private readonly ModularDbContext dbContext;

        public TestController(
            IActionContextAccessor httpContextAccessor,
            ModularDbContext dbContext
        )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
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

        [HttpGet("test/GetDbContextContextId")]
        public IActionResult GetDbContextContextId()
        {
            try
            {
                return Ok(this.dbContext.ContextId);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet("test/GetTestOneEntityInstanceForFakeModuleOne")]
        public IActionResult GetTestOneEntityInstanceForFakeModuleOne()
        {
            try
            {
                var checkEntityType = this.dbContext.Model.GetEntityTypes().Any(w => w.Name == "CSBEF.Module.FakeModuleOne.TestOne");
                if (checkEntityType)
                {
                    return Ok();
                }
                else
                {
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet("test/GetTestTwoEntityInstanceForFakeModuleOne")]
        public IActionResult GetTestOneEntityInstanceForFakeModuleTwo()
        {
            try
            {
                var checkEntityType = this.dbContext.Model.GetEntityTypes().Any(w => w.Name == "CSBEF.Module.FakeModuleOne.TestTwo");
                if (checkEntityType)
                {
                    return Ok();
                }
                else
                {
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet("test/GetTestOneEntityInstanceForFakeModuleTwo")]
        public IActionResult GetTestTwoEntityInstanceForFakeModuleOne()
        {
            try
            {
                var checkEntityType = this.dbContext.Model.GetEntityTypes().Any(w => w.Name == "CSBEF.Module.FakeModuleTwo.TestOne");
                if (checkEntityType)
                {
                    return Ok();
                }
                else
                {
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet("test/GetTestTwoEntityInstanceForFakeModuleTwo")]
        public IActionResult GetTestTwoEntityInstanceForFakeModuleTwo()
        {
            try
            {
                var checkEntityType = this.dbContext.Model.GetEntityTypes().Any(w => w.Name == "CSBEF.Module.FakeModuleTwo.TestTwo");
                if (checkEntityType)
                {
                    return Ok();
                }
                else
                {
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}