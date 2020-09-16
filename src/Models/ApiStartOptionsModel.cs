using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using CSBEF.Concretes;
using CSBEF.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CSBEF.Models
{
    public class ApiStartOptionsModel : IApiStartOptionsModel
    {
        public bool InjectHttpContextAccessor { get; set; } = true;

        public bool ImportingModules { get; set; } = true;

        public bool AddEfDbContext { get; set; } = true;

        public bool DbContextEnableDetailedErrors { get; set; } = true;

        public bool DbContextEnableSensitiveDataLogging { get; set; } = true;

        public ServiceLifetime DbContextLifeTimeType { get; set; } = ServiceLifetime.Transient;

        public QueryTrackingBehavior DbContextQueryTrackingBehavior { get; set; } = QueryTrackingBehavior.NoTracking;

        public bool DbContextLazyLoadingEnabled { get; set; }

        public bool DbContextAutoDetectChangesEnabled { get; set; }

        public CultureInfo DefaultCultureInfo { get; set; } = new CultureInfo("tr-TR");

        public string DefaultMainModuleName { get; set; } = "project";

        public bool AddJwt { get; set; } = true;

        public Action<AuthenticationOptions> JwtAuthenticationOptions { get; set; } = x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        };

        public Action<JwtBearerOptions> JwtJwtBearerOptions { get; set; } = x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CSBEF_JWT_SECRET_KEY")),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            x.Events = new JwtBearerEvents()
            {
                OnTokenValidated = async (context) =>
                {
                    var currentToken = ((JwtSecurityToken)context.SecurityToken).RawData;
                    var eventServiceInstance = context.HttpContext.RequestServices.GetService<IEventService>();
                    var checkTokenStatus = await (await eventServiceInstance.GetEvent("Main", "InComingToken").ConfigureAwait(false)).EventHandler<bool, string>(currentToken).ConfigureAwait(false);

                    if (checkTokenStatus.ErrorInfo.Status)
                    {
                        context.Fail("TokenExpiredOrPassive");
                    }
                },
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub", StringComparison.CurrentCultureIgnoreCase))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        };

        public bool AddUseMvc { get; set; } = true;

        public Action<MvcOptions> MvcBuilder { get; set; } = x =>
        {

        };

        public CompatibilityVersion MvcCompatibilityVersion { get; set; } = CompatibilityVersion.Version_3_0;

        public Action<IMvcBuilder> ReConfigMvcBuilder { get; set; } = mvcBuilder =>
        {
            mvcBuilder.AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            mvcBuilder.AddNewtonsoftJson(x => x.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects);
            mvcBuilder.AddNewtonsoftJson(x => x.SerializerSettings.ContractResolver = new DefaultContractResolver());

            GlobalConfiguration.MvcBuilder = mvcBuilder;
        };

        public MapperConfigurationExpression AutoMapperConfig { get; set; } = new MapperConfigurationExpression();

        public bool UseSignalr { get; set; } = true;

        public Action<HubOptions> SignalrHubOptions { get; set; } = opt =>
        {
            opt.EnableDetailedErrors = false;
            opt.MaximumReceiveMessageSize = null;
        };

        public ServiceLifetime AddTransactionHelperLifetime { get; set; } = ServiceLifetime.Transient;
    }
}