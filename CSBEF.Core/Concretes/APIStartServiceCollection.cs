using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace CSBEF.Core.Concretes {
    public class APIStartServiceCollection {
        #region Dependencies

        private IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnvironment;
        private IServiceCollection _services;
        private StringComparison _stringComparison;

        #endregion Dependencies

        #region Private Properties

        private readonly IList<ModuleInfo> modules = new List<ModuleInfo> ();

        #endregion Private Properties

        public IServiceProvider Init (IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServiceCollection services, ApiStartOptionsModel options = null, StringComparison stringComparison = new StringComparison ()) {
            #region Options Check

            if (options == null)
                options = new ApiStartOptionsModel ();

            #endregion Options Check

            _stringComparison = stringComparison;

            #region Transfer Dependencies

            _configuration = configuration ??
                throw new ArgumentNullException (nameof (configuration));
            _hostingEnvironment = hostingEnvironment ??
                throw new ArgumentNullException (nameof (hostingEnvironment));
            _services = services ??
                throw new ArgumentNullException (nameof (services));

            #endregion Transfer Dependencies

            #region HttpContextAccessor

            services.AddHttpContextAccessor ();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor> ();

            #endregion HttpContextAccessor

            #region Load Modules

            LoadInstalledModules ();

            #endregion Load Modules

            #region DbContext

            if (options.UseEntityFramework) {
                _services.AddDbContext<ModularDbContext> (opt => {
                    switch (configuration["AppSettings:DBSettings:Provider"]) {
                        case "mssql":
                            opt.UseSqlServer (configuration["AppSettings:DBSettings:ConnectionString"]);
                            break;

                        case "mysql":
                            opt.UseMySQL (configuration["AppSettings:DBSettings:ConnectionString"]);
                            break;

                        case "postgresql":
                            opt.UseNpgsql (configuration["AppSettings:DBSettings:ConnectionString"]);
                            break;
                    }

                    opt.EnableDetailedErrors (options.DbContextEnableDetailedErrors);
                    opt.EnableSensitiveDataLogging (options.DbContextEnableSensitiveDataLogging);
                }, options.DbContextLifeTimeType);
            }

            #endregion DbContext

            #region Install JWT Settings

            if (options.UseJwt) {
                var JWTSecretKey = Encoding.ASCII.GetBytes (_configuration["AppSettings:JWTSettings:SecretCode"]);
                _services.AddAuthentication (x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer (x => {
                    x.RequireHttpsMetadata = options.JwtJwtBearerRequireHttpsMetadata;
                    x.SaveToken = options.JwtJwtBearerSaveToken;
                    x.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = options.JwtJwtBearerTokenValidationParametersValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey (JWTSecretKey),
                        ValidateIssuer = options.JwtJwtBearerTokenValidationParametersValidateIssuer,
                        ValidateAudience = options.JwtJwtBearerTokenValidationParametersValidateAudience
                    };
                    x.Events = new JwtBearerEvents () {
                        OnTokenValidated = async (context) => {
                                try {
                                    var job = Task.Run (() => {
                                        var currentToken = ((JwtSecurityToken) context.SecurityToken).RawData;
                                        var eventServiceInstance = context.HttpContext.RequestServices.GetService<IEventService> ();
                                        var checkTokenStatus = eventServiceInstance.GetEvent ("Main", "InComingToken").EventHandler<bool, string> (currentToken);
                                        if (checkTokenStatus.ErrorInfo.Status || !checkTokenStatus.Result) {
                                            context.Fail ("TokenExpiredOrPassive");
                                        }
                                    });

                                    await job.ConfigureAwait (false);
                                } catch (CustomException ex) {
                                    context.Fail (ex);
                                }
                            },
                            OnMessageReceived = context => {
                                try {
                                    var accessToken = context.Request.Query["access_token"];

                                    var path = context.HttpContext.Request.Path;
                                    if (!string.IsNullOrEmpty (accessToken) && path.StartsWithSegments ("/signalr", _stringComparison)) {
                                        context.Token = accessToken;
                                    }
                                    return Task.CompletedTask;
                                } catch (CustomException) {
                                    return Task.CompletedTask;
                                }
                            }
                    };
                });
            }

            #endregion Install JWT Settings

            #region Install MVC Settings

            var mvcBuilder = _services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_3_0);
            if (options.CustomMvcBuilder != null) {
                mvcBuilder = options.CustomMvcBuilder (mvcBuilder);
            } else if (options.JsonOptionsUsing) {
                mvcBuilder.AddNewtonsoftJson (x => x.SerializerSettings.ReferenceLoopHandling = options.JsonOptionsReferenceLoopHandling);
                mvcBuilder.AddNewtonsoftJson (x => x.SerializerSettings.PreserveReferencesHandling = options.JsonOptionsPreserveReferencesHandling);
                mvcBuilder.AddNewtonsoftJson (x => x.SerializerSettings.ContractResolver = options.JsonOptionsContractResolver);
            }

            #endregion Install MVC Settings

            #region Run Process : ModuleInitializer, ModuleEventsAddInitializer, AutoMapperInitializer

            if (options.UseAutomapper) {
                var autoMapperConfig = options.AutoMapperConfig;

                foreach (var module in modules) {
                    mvcBuilder.AddApplicationPart (module.Assembly);

                    var moduleInitializerType = module.Assembly.GetTypes ().Where (x => typeof (IModuleInitializer).IsAssignableFrom (x)).FirstOrDefault ();
                    if (moduleInitializerType != null && moduleInitializerType != typeof (IModuleInitializer)) {
                        var moduleInitializer = (IModuleInitializer) Activator.CreateInstance (moduleInitializerType);
                        moduleInitializer.Init (_services);
                    }

                    var moduleMapperProfileType = module.Assembly.GetTypes ().Where (x => typeof (Profile).IsAssignableFrom (x)).FirstOrDefault ();
                    var moduleMapperProfileTypeInstance = (Profile) Activator.CreateInstance (moduleMapperProfileType);
                    autoMapperConfig.AddProfile (moduleMapperProfileTypeInstance);
                }

                var autoMapperMappingConfig = new MapperConfiguration (autoMapperConfig);

                IMapper mapper = autoMapperMappingConfig.CreateMapper ();
                _services.AddSingleton (mapper);
            }

            #endregion Run Process : ModuleInitializer, ModuleEventsAddInitializer, AutoMapperInitializer

            #region SignalR

            if (options.UseSignalR) {
                _services.AddSignalR (opt => {
                    opt.EnableDetailedErrors = options.SignalREnableDetailedErrors;
                });

                switch (options.ModuleInterfacesIHubNotificationServiceLifeTime) {
                    case ServiceLifetime.Scoped:
                        services.AddScoped<IHubNotificationService, HubNotificationService> ();
                        break;

                    case ServiceLifetime.Singleton:
                        services.AddSingleton<IHubNotificationService, HubNotificationService> ();
                        break;

                    case ServiceLifetime.Transient:
                        services.AddTransient<IHubNotificationService, HubNotificationService> ();
                        break;
                }

                switch (options.ModuleInterfacesIHubSyncDataServiceLifeTime) {
                    case ServiceLifetime.Scoped:
                        services.AddScoped<IHubSyncDataService, HubSyncDataService> ();
                        break;

                    case ServiceLifetime.Singleton:
                        services.AddSingleton<IHubSyncDataService, HubSyncDataService> ();
                        break;

                    case ServiceLifetime.Transient:
                        services.AddTransient<IHubSyncDataService, HubSyncDataService> ();
                        break;
                }
            }

            #endregion SignalR

            #region Run Process : ModuleEventsAddInitializer & ModuleEventsJoinInitializer

            foreach (var module in modules) {
                var moduleEventsAddInitializerType = module.Assembly.GetTypes ().Where (x => typeof (IModuleEventsAddInitializer).IsAssignableFrom (x)).FirstOrDefault ();
                if (moduleEventsAddInitializerType != null && moduleEventsAddInitializerType != typeof (IModuleEventsAddInitializer)) {
                    switch (options.ModuleInterfacesIModuleEventsAddInitializerLifeTime) {
                        case ServiceLifetime.Scoped:
                            services.AddScoped (typeof (IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton (typeof (IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                            break;

                        case ServiceLifetime.Transient:
                            services.AddTransient (typeof (IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                            break;
                    }
                }
            }

            foreach (var module in modules) {
                var moduleEventsJoinInitializerType = module.Assembly.GetTypes ().Where (x => typeof (IModuleEventsJoinInitializer).IsAssignableFrom (x)).FirstOrDefault ();
                if (moduleEventsJoinInitializerType != null && moduleEventsJoinInitializerType != typeof (IModuleEventsJoinInitializer)) {
                    switch (options.ModuleInterfacesIModuleEventsJoinInitializerLifeTime) {
                        case ServiceLifetime.Scoped:
                            services.AddScoped (typeof (IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton (typeof (IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                            break;

                        case ServiceLifetime.Transient:
                            services.AddTransient (typeof (IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                            break;
                    }
                }
            }

            #endregion Run Process : ModuleEventsAddInitializer & ModuleEventsJoinInitializer

            #region Main Events Initializer

            switch (options.ModuleInterfacesMainIModuleEventsAddInitializerLifeTime) {
                case ServiceLifetime.Scoped:
                    _services.AddScoped<IModuleEventsAddInitializer, MainEventsAddInitializer> ();
                    break;

                case ServiceLifetime.Singleton:
                    _services.AddSingleton<IModuleEventsAddInitializer, MainEventsAddInitializer> ();
                    break;

                case ServiceLifetime.Transient:
                    _services.AddTransient<IModuleEventsAddInitializer, MainEventsAddInitializer> ();
                    break;
            }

            #endregion Main Events Initializer

            #region EventService

            switch (options.ModuleInterfacesIEventServiceLifeTime) {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IEventService, EventService> ();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<IEventService, EventService> ();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<IEventService, EventService> ();
                    break;
            }

            #endregion EventService

            #region Transaction Helper

            switch (options.ModuleInterfacesITransactionHelperLifeTime) {
                case ServiceLifetime.Scoped:
                    services.AddScoped<ITransactionHelper, TransactionHelper> ();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<ITransactionHelper, TransactionHelper> ();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<ITransactionHelper, TransactionHelper> ();
                    break;
            }

            #endregion Transaction Helper

            #region Build Service Provider

            var sp = services.BuildServiceProvider ();

            #endregion Build Service Provider

            GlobalConfiguration.ApiStartOptions = options;

            return sp;
        }

        private void LoadInstalledModules () {
            if (!Directory.Exists (Path.Combine (_hostingEnvironment.ContentRootPath, "Modules")))
                return;

            var moduleRootFolder = new DirectoryInfo (Path.Combine (_hostingEnvironment.ContentRootPath, "Modules"));
            var moduleFolders = moduleRootFolder.GetDirectories ();

            foreach (var moduleFolder in moduleFolders) {
                var binFolder = new DirectoryInfo (Path.Combine (moduleFolder.FullName, "bin"));
                if (!binFolder.Exists) {
                    continue;
                }

                foreach (var file in binFolder.GetFileSystemInfos ("*.dll", SearchOption.AllDirectories)) {
                    Assembly assembly = null;
                    try {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath (file.FullName);
                    } catch (FileLoadException ex) {
                        if (ex.Message == "Assembly with same name is already loaded")
                            assembly = Assembly.Load (new AssemblyName (Path.GetFileNameWithoutExtension (file.Name)));
                        else
                            throw;
                    }

                    if (assembly.FullName.Contains (moduleFolder.Name, _stringComparison))
                        modules.Add (new ModuleInfo { Name = moduleFolder.Name, Assembly = assembly, Path = moduleFolder.FullName });
                }
            }

            GlobalConfiguration.Modules.Clear ();
            foreach (var module in modules)
                GlobalConfiguration.Modules.Add (module);
        }
    }
}