using System;
using System.Globalization;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Interfaces
{
    public interface IApiStartOptionsModel
    {
        bool InjectHttpContextAccessor { get; set; }

        bool ImportingModules { get; set; }

        bool AddEfDbContext { get; set; }

        bool DbContextEnableDetailedErrors { get; set; }

        bool DbContextEnableSensitiveDataLogging { get; set; }

        ServiceLifetime DbContextLifeTimeType { get; set; }

        QueryTrackingBehavior DbContextQueryTrackingBehavior { get; set; }

        bool DbContextLazyLoadingEnabled { get; set; }

        bool DbContextAutoDetectChangesEnabled { get; set; }

        CultureInfo DefaultCultureInfo { get; set; }

        string DefaultMainModuleName { get; set; }

        bool AddJwt { get; set; }

        Action<AuthenticationOptions> JwtAuthenticationOptions { get; set; }

        Action<JwtBearerOptions> JwtJwtBearerOptions { get; set; }

        bool AddUseMvc { get; set; }

        Action<MvcOptions> MvcBuilder { get; set; }

        CompatibilityVersion MvcCompatibilityVersion { get; set; }

        Action<IMvcBuilder> ReConfigMvcBuilder { get; set; }

        MapperConfigurationExpression AutoMapperConfig { get; set; }

        bool UseSignalr { get; set; }

        Action<HubOptions> SignalrHubOptions { get; set; }

        ServiceLifetime AddTransactionHelperLifetime { get; set; }
    }
}