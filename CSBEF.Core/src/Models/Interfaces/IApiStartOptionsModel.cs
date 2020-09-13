using System;
using System.Globalization;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF Starter.Start ayarları için kullanılan modeldir.
    /// </summary>
    public interface IApiStartOptionsModel
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Modüllerin HttpContext erişimine ihtiyacı olabiliyor. Bu ve benzeri ihtiyaçlar için bu instance'ın provider'a eklenmesi gerekiyor.
        /// Fakat özel durumlarda bu inject'in yapılması istenmeyebilir. Bu nedenle bu ayar eklenmiştir. Değeri "false" olursa bu instance provider'a eklenmez.
        /// </summary>
        bool InjectHttpContextAccessor { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Modül kütüphanelerinin içeri aktarılması işleminin yapılma durumu ile ilgili ayardır.
        /// Normal şartlarda tüm modül kütüphanelerinin içeri aktarılması gerekir. Fakat özel bir durumdan dolayı bu adımın atlanması istenebilir.
        /// Bu adımın atlanması demek, herhangi bir modülün içeri aktarılmaması anlamına gelir.
        /// </summary>
        bool ImportingModules { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Entity Framework ORM için DbContext instance eklenmesinin yapılma durumu ile ilgili ayardır.
        /// Bu ayar kapatıldığında uygulamaya bir DbContext eklenmez.
        /// </summary>
        bool AddEfDbContext { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// DBContext oluşturulurken ayarlanan "DbContextOptionsBuilder.EnableDetailedErrors" ayarı. 
        /// </summary>
        bool DbContextEnableDetailedErrors { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// DBContext oluşturulurken ayarlanan "DbContextOptionsBuilder.EnableSensitiveDataLogging" ayarı. 
        /// </summary>
        /// <value></value>
        bool DbContextEnableSensitiveDataLogging { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// DbContext için gereken Lifetime bilgisidir.
        /// Önerilen ayar "ServiceLifetime.Transient" şeklindedir. Çünkü önerilen db kullanımı asenkron şeklindedir.
        /// Asenkron kullanımlarda diğer yaşam döngüleri sorun çıkarmaktadır.
        /// </summary>
        ServiceLifetime DbContextLifeTimeType { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// DbContext içerisindeki "ChangeTracker.QueryTrackingBehavior" ayarıdır.
        /// </summary>
        QueryTrackingBehavior DbContextQueryTrackingBehavior { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// DbContext içerisindeki "ChangeTracker.LazyLoadingEnabled" ayarıdır.
        /// </summary>
        bool DbContextLazyLoadingEnabled { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// DbContext içerisindeki "ChangeTracker.AutoDetectChangesEnabled" ayarıdır.
        /// </summary>
        bool DbContextAutoDetectChangesEnabled { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Entegrasyon aşamasında çalıştırılan metotlar içerisinde string işlemleri yer alıyor. Bu kısımlarda kod kalitesi için kullanılacak culture isteniyor.
        /// Bu talebi karşılamak adına belirtilmesi gereken culture bilgisidir.
        /// Bunun için varsayılan culture bilgisi "tr-TR" şeklindedir.
        /// </summary>
        CultureInfo DefaultCultureInfo { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Poco model ile tablo adı eşleştirmesinde, ön ek kuralını görmezden gelmesi için kullanılacak ana modül adıdır.
        /// Burada belirtilen isme sahip modülün tablolarında önek aranmaz.
        /// 
        /// Örneğin; bu kısımda "Project" bilgisi yer alıyorsa; "UserManagement" modülündeki "User" tablosunun veri tabanındaki tablo adı "UserManagement_User"
        /// şeklinde olması gerekir. Ancak "Project" modülündeki "Staff" tablosunun adı "Project_Staff" yerine "Staff" olabilir.
        /// Eğer burada belirtilen modülün içerisindeki tablolara önek koyulmuşsa, CSBEF bu ilişkiyi kuramayacaktır. Çünkü burada belirtilen isim ile
        /// ilgili modüle önek kural esnetmesi yapılacağından, ilgili tablo isimleri eşleşmeyecektir.
        /// </summary>
        string DefaultMainModuleName { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// JWT yapısının engre edilme durumudur.
        /// </summary>
        bool AddJwt { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// JWT entegrasyonunda kullanılmaktadır.
        /// "services.AddAuthentication" için belirtilecek action metottur.
        /// </summary>
        Action<AuthenticationOptions> JwtAuthenticationOptions { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// JWT entegrasyonunda kullanılmaktadır.
        /// "services.AddJwtBearer" için belirtilecek action metottur.
        /// </summary>
        /// <value></value>
        Action<JwtBearerOptions> JwtJwtBearerOptions { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Normal şartlarda startup kısmında ServiceProvider içerisine "AddMvc" şeklinde MVC eklentisi yapılmalıdır.
        /// Fakat bu eklentinin birçok özelleştirilmiş ayarı yapılabilir. Bunların tümü için CSBEF entegrasyonu geliştiriciye imkan sağlamaktadır.
        /// Yine de buna rağmen geliştirici "o işi ben halledeyim sen dokunma" diyebilir. Bu nedenle bu ayar eklenmiştir.
        /// </summary>
        bool AddUseMvc { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// API geliştiricisi tarafından MVC ayarlarının yapılabilmesini sağlayan action'dır.
        /// Eğer "AddUseMvc" true değilse, bu action kullanılmayacaktır.
        /// </summary>
        Action<MvcOptions> MvcBuilder { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// "UseMvc" ile birlikte kullanılacak ve dotnet core sürümünü belirten ayardır.
        /// </summary>
        CompatibilityVersion MvcCompatibilityVersion { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// "UseMvc" ile elde edilen IMvcBuilder nesnesi üzerinde API geliştiricisinin işlem yapabilmesi için oluşturulmuş action'dır.
        /// </summary>
        Action<IMvcBuilder> ReConfigMvcBuilder { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// AutoMapper ayarlarıdır.
        /// </summary>
        MapperConfigurationExpression AutoMapperConfig { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// CSBEF içerisinde SignalR desteği bulunmaktadır.
        /// Modüllerin ortak kullanacağı ve özellikle Vue gibi uygulamalarda, yeni eklenen, güncellenen yada silinen kayıtların anlık olarak tüm client'lere bildirilmesi sağlanmaktadır.
        /// Modül geliştiricisi isterse bu özelliği kullanarak dinamik ve modern bir yapı oluşturabileceği gibi, Chat ve benzeri uygulamalarda geliştirebilir.
        /// Bu ayar, SignalR desteğinin kullanılma durumunu belirtmektedir.
        /// </summary>
        bool UseSignalr { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// SignalR yapılandırılmasında kullanılacak action'dır.
        /// </summary>
        Action<HubOptions> SignalrHubOptions { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Eklenecek olan TransactionHelper desteğinin yaşam döngüsü ayarıdır.
        /// Önerilen ayar ServiceLifetime.Transient şeklindedir.
        /// </summary>
        /// <value></value>
        ServiceLifetime AddTransactionHelperLifetime { get; set; }
    }
}