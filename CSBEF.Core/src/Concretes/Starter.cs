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
using System.Linq;
using CSBEF.Core.Models.Interfaces;
using AutoMapper;
using CSBEF.Core.Concretes;

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
                "JwtIntegrationBefore",
                "JwtIntegrationAfter",
                "AddMvcBefore",
                "AddMvcAfter",
                "RunModuleInitializersBefore",
                "RunModuleInitializersAfter",
                "RunAutoMapperInitializersBefore",
                "RunAutoMapperInitializerAfter",
                "RunSignalRInitializerBefore",
                "RunSignalRInitializerAfter",
                "RunModuleEventsAddInitializersBefore",
                "RunModuleEventsAddInitializersAfter",
                "RunModuleEventsJoinInitializersBefore",
                "RunModuleEventsJoinInitializersAfter",
                "RunMainEventsAddInitializersBefore",
                "RunMainEventsAddInitializersAfter",
                "AddEventServiceBefore",
                "AddEventServiceAfter",
                "AddTransactionHelperBefore",
                "AddTransactionHelperAfter",
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

            #region Adding DbContext for Entity Framework 

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("AddEfDbContextBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result && this.options.AddEfDbContext)
            {
                AddEfDbContext();
            }
            await IntegrationEventBus.TriggerEvent("AddEfDbContextAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region JWT integration 

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("JwtIntegrationBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result && this.options.AddJwt)
            {
                JwtIntegration();
            }
            await IntegrationEventBus.TriggerEvent("JwtIntegrationAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region MVC Integration

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("AddMvcBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result && this.options.AddUseMvc)
            {
                MvcIntegration();
            }
            await IntegrationEventBus.TriggerEvent("AddMvcAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: ModuleInitializer

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("RunModuleInitializersBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                RunModuleInitializers();
            }
            await IntegrationEventBus.TriggerEvent("RunModuleInitializersAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: AutoMapperInitializer

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("RunAutoMapperInitializersBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                RunAutoMapperInitializers();
            }
            await IntegrationEventBus.TriggerEvent("RunAutoMapperInitializerAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: SignalRInitializer

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("RunSignalRInitializerBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result && options.UseSignalr)
            {
                RunSignalRInitializer();
            }
            await IntegrationEventBus.TriggerEvent("RunSignalRInitializerAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: ModuleEventsAddInitializer

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("RunModuleEventsAddInitializersBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                RunModuleEventsAddInitializers();
            }
            await IntegrationEventBus.TriggerEvent("RunModuleEventsAddInitializersAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: ModuleEventsAddInitializer

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("RunModuleEventsJoinInitializersBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                RunModuleEventsJoinInitializers();
            }
            await IntegrationEventBus.TriggerEvent("RunModuleEventsJoinInitializersAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: MainEventsAddInitializer

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("RunMainEventsAddInitializersBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                RunMainEventsJoinInitializers();
            }
            await IntegrationEventBus.TriggerEvent("RunMainEventsAddInitializersAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: Add EventService Instance

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("AddEventServiceBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                AddEventService();
            }
            await IntegrationEventBus.TriggerEvent("AddEventServiceAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

            #region Run Process: Add TransactionHelper Instance

            integrationEventReturn = await IntegrationEventBus.TriggerEvent("AddTransactionHelperBefore", integrationEventArgs).ConfigureAwait(false);
            if (integrationEventReturn.Result)
            {
                AddTransactionHelper();
            }
            await IntegrationEventBus.TriggerEvent("AddTransactionHelperAfter", integrationEventArgs).ConfigureAwait(false);

            #endregion

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
        /// TODO: To be translated into English
        /// API uygulamasına modüler Entity Framework DbContext eklenmesini sağlayan ve bu DbContext'in modüler olarak bir araya gelmesini sağlayan metottur.
        /// </summary>
        private void AddEfDbContext()
        {
            var provider = configuration["AppSettings:DBSettings:Provider"];
            var connectionString = configuration["AppSettings:DBSettings:ConnectionString"];

            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new Exception("\"AppSettings:DBSettings:Provider\" information not found");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("\"AppSettings:DBSettings:ConnectionString\" information not found");
            }

            services.AddDbContext<ModularDbContext>(opt =>
            {
                switch (provider)
                {
                    case "mssql":
                        opt.UseSqlServer(connectionString);
                        break;

                    case "mysql":
                        opt.UseMySQL(connectionString);
                        break;

                    case "postgresql":
                        opt.UseNpgsql(connectionString);
                        break;
                }

                opt.EnableDetailedErrors(GlobalConfiguration.ApiStartOptions.DbContextEnableDetailedErrors);
                opt.EnableSensitiveDataLogging(GlobalConfiguration.ApiStartOptions.DbContextEnableSensitiveDataLogging);
            }, GlobalConfiguration.ApiStartOptions.DbContextLifeTimeType);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// JWT entegrasyonunu yapan yardımcı metottur.
        /// </summary>
        private void JwtIntegration()
        {
            services
                .AddAuthentication(options.JwtAuthenticationOptions)
                .AddJwtBearer(options.JwtJwtBearerOptions);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// API uygulamasına "AddMvc" ile mvc eklentisi yapar.
        /// </summary>
        private void MvcIntegration()
        {
            var mvcBuilder = services.AddMvc(options.MvcBuilder).SetCompatibilityVersion(options.MvcCompatibilityVersion);
            options.ReConfigMvcBuilder(mvcBuilder);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Tüm modüllerin ServiceProvider içerisine kendi instance tanımlarını yapmalarını sağlar.
        /// Bunun için tüm modüllerdeki IModuleInitializer sınıflarındaki "Init" metotlarını tetikler.
        /// </summary>
        private void RunModuleInitializers()
        {
            foreach (var module in GlobalConfiguration.Modules)
            {
                if (options.AddUseMvc)
                {
                    // TODO: To be translated into English
                    // Modül içerisindeki controller ve view'ların API uygulamasına eklenmesi sağlanıyor.
                    GlobalConfiguration.MvcBuilder.AddApplicationPart(module.Assembly);
                }

                // TODO: To be translated into English
                // Ele alınan modülün assembly'si içerisindeki IModuleInitializer sınıfına ServiceProvider gönderiliyor ve kendi instance tanımlarını yapması bekleniyor.
                var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleInitializerType != null && moduleInitializerType != typeof(IModuleInitializer))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services);
                }
            }
        }

        /// <summary>
        /// TODO: To be translated into English
        /// CSBEF modüler AutoMapper desteği sunmaktadır.
        /// Modüller kendi AutoMapper profillerini, API uygulaması için CSBEF tarafından birleştirilerek oluşturulan AutoMapper nesnesine ekleyebilirler.
        /// AutoMapper için kullanılacak olan Mapper, API uygulaması içerisinde kullanılabilir ve singleton olarak provider'a eklenmektedir.
        /// </summary>
        private void RunAutoMapperInitializers()
        {
            var autoMapperConfig = options.AutoMapperConfig;

            foreach (var module in GlobalConfiguration.Modules)
            {
                var moduleMapperProfileType = module.Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).FirstOrDefault();
                var moduleMapperProfileTypeInstance = (Profile)Activator.CreateInstance(moduleMapperProfileType);
                autoMapperConfig.AddProfile(moduleMapperProfileTypeInstance);
            }

            var autoMapperMappingConfig = new MapperConfiguration(autoMapperConfig);
            IMapper mapper = autoMapperMappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// API uygulamasına SignalR desteği için gerekli ayarlamaları yapar.
        /// </summary>
        private void RunSignalRInitializer()
        {
            services.AddSignalR(options.SignalrHubOptions);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Modüllerin kendi event tanımlarını event listesine eklemesini sağlar.
        /// </summary>
        private void RunModuleEventsAddInitializers()
        {
            foreach (var module in GlobalConfiguration.Modules)
            {
                var moduleEventsAddInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleEventsAddInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleEventsAddInitializerType != null && moduleEventsAddInitializerType != typeof(IModuleEventsAddInitializer))
                {
                    services.AddTransient(typeof(IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                }
            }
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Modüllerden toplanılan event tanımlarıyla oluşturulmuş event listesi, bu metot ile yine tüm modüllere sunulur ve
        /// modüllerin istedikleri event'lara subscribe olmaları sağlanır.
        /// /// </summary>
        private void RunModuleEventsJoinInitializers()
        {
            foreach (var module in GlobalConfiguration.Modules)
            {
                var moduleEventsJoinInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleEventsJoinInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleEventsJoinInitializerType != null && moduleEventsJoinInitializerType != typeof(IModuleEventsJoinInitializer))
                {
                    services.AddTransient(typeof(IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                }
            }
        }

        /// <summary>
        /// TODO: To be translated into English
        /// CSBEF içerisindeki bazı core işlemler için tanımlanmış olan event tanımlarını, modüllerden toplanılarak oluşturulan event listesine ekleyen metottur.
        /// Böylece modüller core event'lara da subscribe olabilmektedir.
        /// </summary>
        private void RunMainEventsJoinInitializers()
        {
            services.AddTransient<IModuleEventsAddInitializer, MainEventsAddInitializer>();
        }

        /// <summary>
        /// TODO: To be translated into English
        /// EventService, EventBus kavramını sağlayan ve modüllerin kullanacakları bir servistir.
        /// EventService instance'ı bu metot ile ServiceProvider içerisine eklenmektedir.
        /// </summary>
        private void AddEventService()
        {
            services.AddSingleton<IEventService, EventService>();
        }

        /// <summary>
        /// TODO: To be translated into English
        /// ServiceProvider içerisine TransactionHelper desteğini ekler.
        /// </summary>
        private void AddTransactionHelper()
        {
            switch (options.AddTransactionHelperLifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<ITransactionHelper, TransactionHelper>();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<ITransactionHelper, TransactionHelper>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<ITransactionHelper, TransactionHelper>();
                    break;
            }
        }

        #endregion
    }
}