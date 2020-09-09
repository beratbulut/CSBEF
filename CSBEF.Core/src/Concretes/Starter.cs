using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CSBEF.Models.Interfaces;
using CSBEF.Models;
using System.Threading.Tasks;
using CSBEF.Helpers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Concretes
{
    /// <summary>
    /// TODO: To be translated into English
    /// 
    /// CSBEF yapısının API uygulamasına entegre olmasını ve tüm modüllerin devreye sokulmasını sağlayan sınıftır.
    /// CSBEF yapısı, API uygulamasındaki ServiceCollection üzerinde çeşitli tanımlamalar yapar. Bu tanımlamalar için öncelikle API uygulamasının içerisine eklenmiş olan
    /// modülleri içeri aktarır. İçeri aktarılmış olan modül kütüphaneleri üzerinden planlanmış olaylar üzerinden entegrasyon sağlar.
    /// Entegrasyonların olaylar üzerinden yapılma amacı; ihtiyaç durumunda dışarıdan bu olaylara dahil olarak süreci manipüle etmektir.
    /// İşleyişle ilgili bir diğer önemli nokta ise;
    /// Her bir entegrasyon adımı için iki olay tanımlanmıştır. Bu olaylar adımı gerçekleştirmeden önce ve sonra tetiklenmektedir.
    /// Tüm olaylar "IReturnModel" kullanılarak dönüş yaptığından, herhangi bir adım pas geçilebilir.
    /// </summary>
    public class Starter
    {
        # region Dependencies

        /*
        TODO: To be translated into English
        Bu kısımda, entegrasyon için gereken bağımlılıklar yer almaktadır. 
        Bu bağımlılıklar, CSBEF entegrasyonu yapılan API uygulamasından elde edilmektedir.
        */

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IServiceCollection services;
        private readonly IApiStartOptionsModel options;
        private readonly IntegrationEventArgs integrationEventArgs;

        #endregion

        #region ctor

        public Starter(
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment,
            IServiceCollection services,
            IApiStartOptionsModel options = null
        )
        {
            // TODO: To be translated into English
            // Gelen parametreleri kontrol ediyoruz ve herhangi biri null gelmişse, 
            // throw ile hata fırlatarak uygulamanın durmasını sağlıyoruz (eğer ayrıca bir hata denetimi yoksa).
            configuration.ThrowIfNull();
            hostingEnvironment.ThrowIfNull();
            services.ThrowIfNull();

            // TODO: To be translated into English
            // Gelen parametreleri içeri aktarıyoruz. Bunların tümü diğer metotlarda kullanılacak.
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.services = services;
            this.options = options;

            // TODO: To be translated into English
            // Normal şartlarda özel ayar vermeye gerek yoktur. Fakat ihtiyaca göre "ApiStartOptionsModel" modeli ile özel ayarlar tanımlanabilir.
            // Eğer ayar yoksa, varsayılan değerlerle yeni bir model oluşturuyor ve içeri aktarıyoruz.
            if (this.options == null)
            {
                this.options = new ApiStartOptionsModel();
            }

            // TODO: To be translated into English
            // Entegrasyon adımlarında tetiklenmesi gereken tüm olay tanımlarını bu kısımda "IntegrationEventBus" içerisine ekliyoruz.
            #region Initializing Integration Event Bus

            // TODO: To be translated into English
            // Tüm tetiklemelerde kullanmak için gereken "IntegrationEventArgs" modelini oluşturuyoruz.
            integrationEventArgs = new IntegrationEventArgs(this.configuration, this.hostingEnvironment, this.services, this.options);

            // TODO: To be translated into English
            // Tüm entegrasyon adımları için gereken olayları "IntegrationEventArgs" içerisindeki listeye ekliyoruz.
            IntegrationEventBus.AddEvents(new List<string>{
                "IntegrationStart",
                "InjectHttpContextAccessorBefore",
                "InjectHttpContextAccessorAfter",
                "ImportingModulesBefore",
                "ImportingModulesAfter",
                "AddEfDbContextBefore",
                "AddEfDbContextAfter",
                "IntegrationEnd"
            });

            #endregion
        }

        #endregion

        #region Public Actions

        /// <summary>
        /// TODO: To be translated into English
        /// Entegrasyonu başlatan metottur.
        /// </summary>
        public async Task<IReturnModel<bool>> Start()
        {
            IReturnModel<bool> integrationEventReturn;

            #region Trigger Integration Start Event

            await IntegrationEventBus.TriggerEvent("IntegrationStart", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region HttpContextAccessor Injection

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("InjectHttpContextAccessorBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result && this.options.InjectHttpContextAccessor)
            {
                InjectHttpContextAccessor();
            }
            await IntegrationEventBus.TriggerEvent("InjectHttpContextAccessorAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Importing Modules

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("ImportingModulesBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result && this.options.ImportingModules)
            {
                ImportingModules();
            }
            await IntegrationEventBus.TriggerEvent("ImportingModulesAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            // #region Adding DbContext for Entity Framework 

            // integrationEventReturn = await IntegrationEventBus.TriggerEvent("AddEfDbContextBefore", integrationEventArgs).ConfigureAwait(false);
            // if (integrationEventReturn.Result && this.options.AddEfDbContext)
            // {
            //     AddEfDbContext();
            // }
            // await IntegrationEventBus.TriggerEvent("AddEfDbContextAfter", integrationEventArgs).ConfigureAwait(false);

            // #endregion

            return new ReturnModel<bool>().SendResult(true);
        }

        #endregion

        #region Private Actions

        /// <summary>
        /// TODO: To be translated into English
        /// ServiceProvider içerisine "HttpContextAccessor" instance'ını inject eden metottur.
        /// </summary>
        private void InjectHttpContextAccessor()
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        /// <summary>
        /// TODO: To be translated into English
        /// CSBEF entegrasyonu için CSBEF modül kütüphanelerini reflection ile içeriye aktaran metottur.
        /// </summary>
        private void ImportingModules()
        {
            if (!Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "Modules")))
                return;

            var moduleRootFolder = new DirectoryInfo(Path.Combine(hostingEnvironment.ContentRootPath, "Modules"));
            var moduleFolders = moduleRootFolder.GetDirectories();

            foreach (var moduleFolder in moduleFolders)
            {
                var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
                if (!binFolder.Exists)
                {
                    continue;
                }

                foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException ex)
                    {
                        if (ex.Message == "Assembly with same name is already loaded")
                            assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));
                        else
                            throw;
                    }

                    if (assembly.FullName.Contains(moduleFolder.Name, StringComparison.CurrentCulture))
                        GlobalConfiguration.Modules.Add(new ModuleInfo { Name = moduleFolder.Name, Assembly = assembly, Path = moduleFolder.FullName });
                }
            }
        }

        /// <summary>
        /// API uygulamasına modüler Entity Framework DbContext eklenmesini sağlayan ve bu DbContext'in modüler olarak bir araya gelmesini sağlayan metottur.
        /// </summary>
        private void AddEfDbContext()
        {
            var provider = configuration["AppSettings:DBSettings:Provider"];

            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new Exception("\"AppSettings:DBSettings:Provider\" information not found");
            }

            services.AddDbContext<ModularDbContext>(opt =>
            {
                switch (configuration["AppSettings:DBSettings:Provider"])
                {
                    case "mssql":
                        opt.UseSqlServer(configuration["AppSettings:DBSettings:ConnectionString"]);
                        break;

                    case "mysql":
                        opt.UseMySQL(configuration["AppSettings:DBSettings:ConnectionString"]);
                        break;

                    case "postgresql":
                        opt.UseNpgsql(configuration["AppSettings:DBSettings:ConnectionString"]);
                        break;
                }

                opt.EnableDetailedErrors(GlobalConfiguration.ApiStartOptions.DbContextEnableDetailedErrors);
                opt.EnableSensitiveDataLogging(GlobalConfiguration.ApiStartOptions.DbContextEnableSensitiveDataLogging);
            }, GlobalConfiguration.ApiStartOptions.DbContextLifeTimeType);
        }

        #endregion
    }
}