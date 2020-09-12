using CSBEF.Concretes;
using CSBEF.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSBEF.Core.Tests.ApiEnv
{
    /// <summary>
    /// TODO: To be translated into English
    /// Unit test içerisinde, test amaçlı bir API uygulaması ayağa kaldırılıyor.
    /// Bu sayede entegrasyon testleri de gerçek bir API uygulaması üzerinden yapılmış oluyor.
    /// Bu Startup, bu API uygulamasını ayağa kaldırmak için kullanılıyor.
    /// </summary>
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
            // TODO: To be translated into English
            // Bu kısımda CSBEF entegrasyonu gerçekleştiriliyor.
            // Eğer gerek olursa, API uygulamasına özel diğer tanımlar yapılabilir ancak bunların CSBEF entegrasyon kodlarından sonra yapılması gerekiyor.
            // Çünkü CSBEF gerek DbContext gerekse de diğer entegrasyon işlemleri eklenecek diğer şeylere etkide bulunabilir.
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

            // TODO: To be translated into English
            // API test için oluşturulan test API'si olduğundan, her seferinde veri tabanı içeriği silinerek yeniden oluşturuluyor.
            // Bu oluşturma işleminde de map'ler kullanılıyor.
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ModularDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
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