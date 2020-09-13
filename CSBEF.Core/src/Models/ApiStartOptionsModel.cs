using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using CSBEF.Concretes;
using CSBEF.Core.Models.Interfaces;
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
                /// <summary>
                /// TODO: To be translated into English
                /// API erişimlerinde yapılacak token kontrolüdür.
                /// </summary>
                OnTokenValidated = (context) =>
                {
                    // TODO: To be translated into English
                    // Gelen token bilgisini değişkene aktarıyoruz. Çünkü bu bilgiyi kullanacağız.
                    var currentToken = ((JwtSecurityToken)context.SecurityToken).RawData;

                    // TODO: To be translated into English
                    // Singleton olarak oluşturulan IEventService instance'ına erişiyoruz.
                    var eventServiceInstance = context.HttpContext.RequestServices.GetService<IEventService>();

                    // TODO: To be translated into English
                    // Tüm modüllerce bilinen ve main event olan "InComingToken" event'ını tetikliyoruz.
                    // CSBEF içerisinde entegre kullanıcı denetimi modülü yer almamaktadır.
                    // Ancak JWT ana entegrasyonda (yani bu kısımda) yüklenen bir özelliktir.
                    // Bu nedenle modül geliştiricileri tarafından üretilecek bir kullanıcı denetimi modülünün, bir token geldiğinde süreci ele alabilmesi için bu kısma özel bir main event oluşturulmuştur.
                    var checkTokenStatus = eventServiceInstance.GetEvent("Main", "InComingToken").EventHandler<bool, string>(currentToken);

                    // TODO: To be translated into English
                    // "InComingToken" event handler'dan hata dönerse token geçersiz demektir. Bu nedenle de "TokenExpiredOrPassive" hatası döndürüyoruz.
                    // Bu hata dönme durumunuda şu şekilde açıklayabiliriz;
                    // Eğer API uygulaması içerisinde kullanıcı denetimi yapan bir modül yoksa, bu event'ın subscribe'ı da yok demektir.
                    // Subscribe'ı olmayan event'lar handler edildiğinde, geriye hata dönülmez.
                    // Eğer geriye hata dönüyorsa, bunu subscribe olan bir modül sağlıyor demektir.
                    // Peki hangi durumda modül hata döndürür?
                    // Modül gelen token bilgisini doğrulama sürecine sokar ve geçersiz buluyorsa hata döndürür.
                    // Örnek olarak; modül, üretilen token bilgilerini cihazlara göre kaydeden ve ilgili kullanıcıyla ilişkilendiren bir yöntem uyguluyorsa,
                    // gelen token bilgisini bu listede arar. Eğer bulamazsa "bu token'ı ben üretmemişim, o zaman geçerli değildir" der ve hata döndürür.
                    // Yada kullanıcılara "cihazdaki oturumu kapat" şeklinde bir özellik sunduğu için bu yapıyı sağlıyorsa ve kullanıcı bu oturumu kapatmak istediğinde
                    // ilgili token kaydını siliyorsa veya pasif duruma geçiyorsa, gelen token'ın artık geçersiz olduğunu belirtmek için de yine hata döndürebilir.
                    if (checkTokenStatus.ErrorInfo.Status)
                    {
                        context.Fail("TokenExpiredOrPassive");
                    }

                    return Task.CompletedTask;
                },
                /// <summary>
                /// TODO: To be translated into English
                /// CSBEF hub desteği de sağlamaktadır.
                /// Hub erişimlerinde token denetimi sağlanabilir.
                /// Bu kısımda bu erişim için gereken işlem gerçekleştirilmektedir.
                /// </summary>
                OnMessageReceived = context =>
                {
                    // TODO: To be translated into English
                    /*
                    Hub'ların token denetimi içerisinde işlemlerini gerçekleştirmek istendiğinde bunu sağlamak için gelen token bilgisi,
                    identity'nin analayacağı şekilde yerleştirilir. Bu yerleştirme işlemi bu kısımda yapılmaktadır.
                    */

                    // TODO: To be translated into English
                    // Request parametresi olarak gelen token bilgisi elde ediliyor.
                    var accessToken = context.Request.Query["access_token"];

                    // TODO: To be translated into English
                    // Hub için erişilmek istenen url path'i elde ediliyor.
                    var path = context.HttpContext.Request.Path;

                    // TODO: To be translated into English
                    // Eğer token bilgisi iletilmişse ve hub url'si "/hub" ile başlıyorsa bu denetim devreye girmekte ve context içerisine token bilgisini işlemektedir.
                    // Buradaki "/hub" kontrolünün eklenme amacı; API uygulamasına yapılacak CSBEF yapısı dışındaki geliştirmelerle başka hub'lar da yerleştirilebilir
                    // ve bu hub'larda bu tarz bir kontrol gerçekleştirilmesi istenmeyebilir. Bunu sağlamak için bu şekilde bir kontrol eklenmiştir.
                    // Bu da demek oluyor ki; CSBEF modülleri içerisinde gelistirilecek hub'ların, startup kısmında endpoint adresleri "/hub" ile başlamalıdır ;)
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