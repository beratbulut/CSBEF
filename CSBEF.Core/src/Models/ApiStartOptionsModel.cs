using System;
using System.Globalization;
using CSBEF.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        public string JwtSecretKey { get; set; } = "CSBEF_JWT_SECRET_KEY";
    }
}