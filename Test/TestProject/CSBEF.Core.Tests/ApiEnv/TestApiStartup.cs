using CSBEF.Concretes;
using CSBEF.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSBEF.Core.Tests.ApiEnv
{
    public class TestApiStartup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;

        public TestApiStartup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var CsbefStarterOptions = new ApiStartOptionsModel
            {

            };
            var CsbefStarter = new Starter(configuration, hostingEnvironment, services, CsbefStarterOptions);
            CsbefStarter.Start().Wait();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}