using System;
using System.Linq;
using CSBEF.Concretes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CSBEF.Core.Tests.ApiEnv
{
    /// <summary>
    /// TODO: To be translated into English
    /// Tüm entegrasyon testleri bu controller içerisinde yer alıyor.
    /// Yeni testler ekleneceği zaman yine bu controller içerisine eklenmesi tavsiye ediliyor.
    /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// ServiceProvider içerisine "HttpContextAccessor" instance'ının entegrasyonunu test etmek için kullanılan action.
        /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// Modül kütüphanelerinin reflection ile içeri aktarılmasını test eden action.
        /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// DbContext instance'ının oluşturulmasını test eden action.
        /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// Test amaçlı yer alan sahte modül 1'in içerisinde yer alan "TestOne" entity modelinin DbContext içerisine entegre edilmesini test eden action.
        /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// Test amaçlı yer alan sahte modül 1'in içerisinde yer alan "TestTwo" entity modelinin DbContext içerisine entegre edilmesini test eden action.
        /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// Test amaçlı yer alan sahte modül 2'in içerisinde yer alan "TestOne" entity modelinin DbContext içerisine entegre edilmesini test eden action.
        /// </summary>
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

        /// <summary>
        /// TODO: To be translated into English
        /// Test amaçlı yer alan sahte modül 2'in içerisinde yer alan "TestTwo" entity modelinin DbContext içerisine entegre edilmesini test eden action.
        /// </summary>
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