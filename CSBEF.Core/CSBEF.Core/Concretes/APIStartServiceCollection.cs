using AutoMapper;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public IServiceProvider Init(IConfiguration configuration, IHostingEnvironment hostingEnvironment, IServiceCollection services)
        {
            #region Transfer Dependencies

            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _services = services;

            #endregion Transfer Dependencies

            #region Load Modules

            LoadInstalledModules();

            #endregion Load Modules

            #region DbContext

            _services.AddDbContext<ModularDbContext>(options =>
            {
                switch (configuration["AppSettings:DBSettings:Provider"])
                {
                    case "mssql":
                        options.UseSqlServer(configuration["AppSettings:DBSettings:ConnectionString"]);
                        break;

                    case "mysql":
                        options.UseMySQL(configuration["AppSettings:DBSettings:ConnectionString"]);
                        break;

                    case "postgresql":
                        options.UseNpgsql(configuration["AppSettings:DBSettings:ConnectionString"]);
                        break;
                }

                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Scoped);

            #endregion DbContext

            #region Install JWT Settings

            var JWTSecretKey = Encoding.ASCII.GetBytes(_configuration["AppSettings:JWTSettings:SecretCode"]);
            _services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(JWTSecretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
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
            //.AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            //.AddJsonOptions(x => x.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects)
            //.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            #endregion Install MVC Settings

            #region Run Process: ModuleInitializer, ModuleEventsAddInitializer, AutoMapperInitializer

            var autoMapperConfig = new AutoMapper.Configuration.MapperConfigurationExpression
            {
                ValidateInlineMaps = false
            };

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

            _services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            #endregion SignalR

            #region Run Process: ModuleEventsAddInitializer & ModuleEventsJoinInitializer

            foreach (var module in modules)
            {
                var moduleEventsAddInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleEventsAddInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleEventsAddInitializerType != null && moduleEventsAddInitializerType != typeof(IModuleEventsAddInitializer))
                {
                    services.AddScoped(typeof(IModuleEventsAddInitializer), moduleEventsAddInitializerType);
                }
            }

            foreach (var module in modules)
            {
                var moduleEventsJoinInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleEventsJoinInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleEventsJoinInitializerType != null && moduleEventsJoinInitializerType != typeof(IModuleEventsJoinInitializer))
                {
                    services.AddScoped(typeof(IModuleEventsJoinInitializer), moduleEventsJoinInitializerType);
                }
            }

            #endregion Run Process: ModuleEventsAddInitializer & ModuleEventsJoinInitializer

            #region Main Events Initializer

            _services.AddScoped<IModuleEventsAddInitializer, MainEventsAddInitializer>();

            #endregion Main Events Initializer

            #region EventService

            services.AddScoped<IEventService, EventService>();

            #endregion EventService

            #region HubNotificationService

            services.AddScoped<IHubNotificationService, HubNotificationService>();

            #endregion HubNotificationService

            #region Transaction Helper

            services.AddScoped<ITransactionHelper, TransactionHelper>();

            #endregion

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