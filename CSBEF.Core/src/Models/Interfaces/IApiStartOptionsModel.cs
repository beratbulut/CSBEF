using System.Globalization;
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
        /// JWT için gerekli olan gizli anahtar değeridir.
        /// Her proje için bu anahtar değiştirilmelidir.
        /// </summary>
        string JwtSecretKey { get; set; }
    }
}