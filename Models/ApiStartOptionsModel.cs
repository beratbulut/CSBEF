using System;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CSBEF.Core.Models {
    public class ApiStartOptionsModel {
        public ServiceLifetime DbContextLifeTimeType { get; set; }
        public bool DbContextEnableDetailedErrors { get; set; }
        public bool DbContextEnableSensitiveDataLogging { get; set; }
        public bool JwtJwtBearerRequireHttpsMetadata { get; set; }
        public bool JwtJwtBearerSaveToken { get; set; }
        public bool JwtJwtBearerTokenValidationParametersValidateIssuerSigningKey { get; set; }
        public bool JwtJwtBearerTokenValidationParametersValidateIssuer { get; set; }
        public bool JwtJwtBearerTokenValidationParametersValidateAudience { get; set; }
        public bool JsonOptionsUsing { get; set; }
        public ReferenceLoopHandling JsonOptionsReferenceLoopHandling { get; set; }
        public PreserveReferencesHandling JsonOptionsPreserveReferencesHandling { get; set; }
        public DefaultContractResolver JsonOptionsContractResolver { get; set; }
        public MapperConfigurationExpression AutoMapperConfig { get; set; }
        public bool SignalREnableDetailedErrors { get; set; }
        public ServiceLifetime ModuleInterfacesIModuleEventsAddInitializerLifeTime { get; set; }
        public ServiceLifetime ModuleInterfacesIModuleEventsJoinInitializerLifeTime { get; set; }
        public ServiceLifetime ModuleInterfacesMainIModuleEventsAddInitializerLifeTime { get; set; }
        public ServiceLifetime ModuleInterfacesIEventServiceLifeTime { get; set; }
        public ServiceLifetime ModuleInterfacesIHubNotificationServiceLifeTime { get; set; }
        public ServiceLifetime ModuleInterfacesIHubSyncDataServiceLifeTime { get; set; }
        public ServiceLifetime ModuleInterfacesITransactionHelperLifeTime { get; set; }
        public bool UseEntityFramework { get; set; }
        public bool UseJwt { get; set; }
        public Func<IMvcBuilder, IMvcBuilder> CustomMvcBuilder { get; set; }
        public bool UseAutomapper { get; set; }
        public bool UseSignalR { get; set; }

        public ApiStartOptionsModel () {
            DbContextLifeTimeType = ServiceLifetime.Scoped;
            DbContextEnableDetailedErrors = true;
            DbContextEnableSensitiveDataLogging = true;
            JwtJwtBearerRequireHttpsMetadata = false;
            JwtJwtBearerSaveToken = true;
            JwtJwtBearerTokenValidationParametersValidateIssuerSigningKey = true;
            JwtJwtBearerTokenValidationParametersValidateIssuer = false;
            JwtJwtBearerTokenValidationParametersValidateAudience = false;
            JsonOptionsUsing = false;
            JsonOptionsReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            JsonOptionsPreserveReferencesHandling = PreserveReferencesHandling.Objects;
            JsonOptionsContractResolver = new DefaultContractResolver ();
            AutoMapperConfig = new MapperConfigurationExpression ();
            SignalREnableDetailedErrors = false;
            ModuleInterfacesIModuleEventsAddInitializerLifeTime = ServiceLifetime.Transient;
            ModuleInterfacesIModuleEventsJoinInitializerLifeTime = ServiceLifetime.Transient;
            ModuleInterfacesMainIModuleEventsAddInitializerLifeTime = ServiceLifetime.Transient;
            ModuleInterfacesIEventServiceLifeTime = ServiceLifetime.Transient;
            ModuleInterfacesIHubNotificationServiceLifeTime = ServiceLifetime.Transient;
            ModuleInterfacesIHubSyncDataServiceLifeTime = ServiceLifetime.Transient;
            ModuleInterfacesITransactionHelperLifeTime = ServiceLifetime.Transient;
            UseEntityFramework = true;
            UseJwt = true;
            UseAutomapper = true;
            UseSignalR = true;
        }
    }
}