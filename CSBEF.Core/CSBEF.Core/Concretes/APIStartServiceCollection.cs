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
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace CSBEF.Core.Concretes
{
    public class APIStartServiceCollection
    {
        #region Dependencies

        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        private IServiceCollection _services;

        #endregion Dependencies

        #region Private Properties

        private readonly IList<ModuleInfo> modules = new List<ModuleInfo>();

        #endregion Private Properties

        public IServiceProvider Init(IConfiguration configuration, IHostingEnvironment hostingEnvironment, IServiceCollection services, ApiStartOptionsModel options = null)
        {
            #region Options Check

            if (options == null)
                options = new ApiStartOptionsModel();

            #endregion Options Check

            #region Transfer Dependencies

            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _services = services;

            #endregion Transfer Dependencies

            #region HttpContextAccessor

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            #endregion HttpContextAccessor

            #region Load Modules

            LoadInstalledModules();

            #endregion Load Modules

            #region DbContext

            _services.AddDbContext<ModularDbContext>(opt =>
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

                opt.EnableDetailedErrors(options.DbContext_EnableDetailedErrors);
                opt.EnableSensitiveDataLogging(options.DbContext_EnableSensitiveDataLogging);
            }, options.DbContext_LifeTimeType);

            #endregion DbContext

            #region Install JWT Settings

            var JWTSecretKey = Encoding.ASCII.GetBytes(_configuration["AppSettings:JWTSettings:SecretCode"]);
            _services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = options.Jwt_JwtBearer_RequireHttpsMetadata;
                x.SaveToken = options.Jwt_JwtBearer_SaveToken;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = options.Jwt_JwtBearer_TokenValidationParameters_ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(JWTSecretKey),
                    ValidateIssuer = options.Jwt_JwtBearer_TokenValidationParameters_ValidateIssuer,
                    ValidateAudience = options.Jwt_JwtBearer_TokenValidationParameters_ValidateAudience
                };
                x.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = async (context) =>
                    {
                        try
                        {
                            var currentToken = ((JwtSecurityToken)context.SecurityToken).RawData;
                            var eventServiceInstance = context.HttpContext.RequestServices.GetService<IEventService>();
                            var checkTokenStatus = await eventServiceInstance.GetEvent("Main", "InComingToken").EventHandler<bool, string>(currentToken);
                            if (checkTokenStatus.Error.Status || !checkTokenStatus.Result)
                            {
                                context.Fail("TokenExpiredOrPassive");
                            }
                        }
                        catch (Exception ex)
                        {
                            context.Fail(ex);
                        }
                    },
                    OnMessageReceived = context =>
                    {
                        try
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/signalr")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                        catch (Exception)
                        {
                            return Task.CompletedTask;
                        }
                    }
                };
            });

            #endregion Install JWT Settings

            #region Install MVC Settings

            var mvcBuilder = _services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            if (options.JsonOptions_Using)
            {
                mvcBuilder.AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = options.JsonOptions_ReferenceLoopHandling);
                mvcBuilder.AddJsonOptions(x => x.SerializerSettings.PreserveReferencesHandling = options.JsonOptions_PreserveReferencesHandling);
                mvcBuilder.AddJsonOptions(x => x.SerializerSettings.ContractResolver = options.JsonOptions_ContractResolver);
            }

            #endregion Install MVC Settings

            #region Run Process: ModuleInitializer, ModuleEventsAddInitializer, AutoMapperInitializer

            var autoMapperConfig = options.AutoMapperConfig;

            foreach (var module in modules)
            {
                mvcBuilder.AddApplicationPart(module.Assembly);

                var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleInitializerType != null && moduleInitializerType != typeof(IModuleInitializer))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(_services);
                }

                var moduleMapperProfileType = module.Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).FirstOrDefault();
                var moduleMapperProfileTypeInstance = (Profile)Activator.CreateInstance(moduleMapperProfileType);
                autoMapperConfig.AddProfile(moduleMapperProfileTypeInstance);
            }

            var autoMapperMappingConfig = new MapperConfiguration(autoMapperConfig);

            IMapper mapper = autoMapperMappingConfig.CreateMapper();
            _services.AddSingleton(mapper);

            #endregion Run Process: ModuleInitializer, ModuleEventsAddInitializer, AutoMapperInitializer

            #region SignalR

            _services.AddSignalR(opt =>
            {
                opt.EnableDetailedErrors = options.SignalR_EnableDetailedErrors;
            });

            #endregion SignalR

            #region Run Process: ModuleEventsAddInitializer & ModuleEventsJoinInitializer

            foreach (var module in modules)
            {
                var moduleEventsAddInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleEventsAddInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleEventsAddInitializerType != null && moduleEventsAddInitializerType != typeof(IModuleEventsAddInitializer))
                {
                    switch (options.ModuleInterfaces_IModuleEventsAddInitializer_LifeTime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(typeof(IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton(typeof(IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                            break;

                        case ServiceLifetime.Transient:
                            services.AddTransient(typeof(IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                            break;
                    }
                }
            }

            foreach (var module in modules)
            {
                var moduleEventsJoinInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleEventsJoinInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleEventsJoinInitializerType != null && moduleEventsJoinInitializerType != typeof(IModuleEventsJoinInitializer))
                {
                    switch (options.ModuleInterfaces_IModuleEventsJoinInitializer_LifeTime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(typeof(IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton(typeof(IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                            break;

                        case ServiceLifetime.Transient:
                            services.AddTransient(typeof(IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                            break;
                    }
                }
            }

            #endregion Run Process: ModuleEventsAddInitializer & ModuleEventsJoinInitializer

            #region Main Events Initializer

            switch (options.ModuleInterfaces_Main_IModuleEventsAddInitializer_LifeTime)
            {
                case ServiceLifetime.Scoped:
                    _services.AddScoped<IModuleEventsAddInitializer, MainEventsAddInitializer>();
                    break;

                case ServiceLifetime.Singleton:
                    _services.AddSingleton<IModuleEventsAddInitializer, MainEventsAddInitializer>();
                    break;

                case ServiceLifetime.Transient:
                    _services.AddTransient<IModuleEventsAddInitializer, MainEventsAddInitializer>();
                    break;
            }

            #endregion Main Events Initializer

            #region EventService

            switch (options.ModuleInterfaces_IEventService_LifeTime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IEventService, EventService>();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<IEventService, EventService>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<IEventService, EventService>();
                    break;
            }

            #endregion EventService

            #region HubNotificationService

            switch (options.ModuleInterfaces_IHubNotificationService_LifeTime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IHubNotificationService, HubNotificationService>();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<IHubNotificationService, HubNotificationService>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<IHubNotificationService, HubNotificationService>();
                    break;
            }

            #endregion HubNotificationService

            #region HubSyncDataService

            switch (options.ModuleInterfaces_IHubSyncDataService_LifeTime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IHubSyncDataService, HubSyncDataService>();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<IHubSyncDataService, HubSyncDataService>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<IHubSyncDataService, HubSyncDataService>();
                    break;
            }

            #endregion HubSyncDataService

            #region Transaction Helper

            switch (options.ModuleInterfaces_ITransactionHelper_LifeTime)
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

            #endregion Transaction Helper

            #region Build Service Provider

            var sp = services.BuildServiceProvider();

            #endregion Build Service Provider

            return sp;
        }

        private void LoadInstalledModules()
        {
            if (!Directory.Exists(Path.Combine(_hostingEnvironment.ContentRootPath, "Modules")))
                return;

            var moduleRootFolder = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "Modules"));
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

                    if (assembly.FullName.Contains(moduleFolder.Name))
                        modules.Add(new ModuleInfo { Name = moduleFolder.Name, Assembly = assembly, Path = moduleFolder.FullName });
                }
            }

            GlobalConfiguration.Modules = modules;
        }
    }
}