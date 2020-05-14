using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CSBEF.Core.Models
{
    public class ApiStartOptionsModel
    {
        public ServiceLifetime DbContext_LifeTimeType { get; set; } = ServiceLifetime.Scoped;
        public bool DbContext_EnableDetailedErrors { get; set; } = true;
        public bool DbContext_EnableSensitiveDataLogging { get; set; } = true;
        public bool Jwt_JwtBearer_RequireHttpsMetadata { get; set; } = false;
        public bool Jwt_JwtBearer_SaveToken { get; set; } = true;
        public bool Jwt_JwtBearer_TokenValidationParameters_ValidateIssuerSigningKey { get; set; } = true;
        public bool Jwt_JwtBearer_TokenValidationParameters_ValidateIssuer { get; set; } = false;
        public bool Jwt_JwtBearer_TokenValidationParameters_ValidateAudience { get; set; } = false;
        public bool JsonOptions_Using { get; set; } = false;
        public ReferenceLoopHandling JsonOptions_ReferenceLoopHandling { get; set; } = ReferenceLoopHandling.Ignore;
        public PreserveReferencesHandling JsonOptions_PreserveReferencesHandling { get; set; } = PreserveReferencesHandling.Objects;
        public DefaultContractResolver JsonOptions_ContractResolver { get; set; } = new DefaultContractResolver();

        public MapperConfigurationExpression AutoMapperConfig { get; set; } = new MapperConfigurationExpression();

        public bool SignalR_EnableDetailedErrors { get; set; } = false;
        public ServiceLifetime ModuleInterfaces_IModuleEventsAddInitializer_LifeTime { get; set; } = ServiceLifetime.Scoped;
        public ServiceLifetime ModuleInterfaces_IModuleEventsJoinInitializer_LifeTime { get; set; } = ServiceLifetime.Scoped;
        public ServiceLifetime ModuleInterfaces_Main_IModuleEventsAddInitializer_LifeTime { get; set; } = ServiceLifetime.Scoped;
        public ServiceLifetime ModuleInterfaces_IEventService_LifeTime { get; set; } = ServiceLifetime.Scoped;
        public ServiceLifetime ModuleInterfaces_IHubNotificationService_LifeTime { get; set; } = ServiceLifetime.Scoped;
        public ServiceLifetime ModuleInterfaces_IHubSyncDataService_LifeTime { get; set; } = ServiceLifetime.Scoped;
        public ServiceLifetime ModuleInterfaces_ITransactionHelper_LifeTime { get; set; } = ServiceLifetime.Scoped;
    }
}