using System.Collections.Generic;
using CSBEF.Models;
using CSBEF.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Concretes
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF Starter, entegrasyon işlemlerini yaparken bazı bilgileri muhafaza eder. 
    /// Bunun nedeni; diğer modüllerin veya diğer iş parçacıklarının bu bilgilere ihtiyaç duyabilme ihtimallerinin olmasıdır.
    /// Bu nedenlerle bu sınıf static olarak oluşturulmuştur.
    /// </summary>
    public static class GlobalConfiguration
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Sisteme yüklenmiş olan modüllerin assambly'lerini tutan collection'dır.
        /// Bu collection çok önemlidir, çünkü modüler yapı bu liste üzerinden işlem yapmaktadır. 
        /// Bu nedenle bu collection'a static olarak her yerden erişilebilmektedir.
        /// </summary>
        public static List<ModuleInfo> Modules { get; } = new List<ModuleInfo>();

        /// <summary>
        /// TODO: To be translated into English
        /// CSBEF entegrasyonu için kullanılan ayarları içeren modelin instance'ıdır.
        /// </summary>
        public static IApiStartOptionsModel ApiStartOptions { get; set; } = new ApiStartOptionsModel();

        /// <summary>
        /// TODO: To be translated into English
        /// API uygulamasına MVC özelliği eklendiğinde elde edilen MvcBuilder nesnesidir.
        /// Option modeli içerisinde varsayılan MVC işlemleri yapılmaktadır.
        /// Bu işlemler içerisinde elde edilen MvcBuilder nesnesi, bu property'ye atanmaktadır.
        /// Eğer option kısmındaki varsayılan bu action değiştirilirse, muhakkak kendi action'ınız içerisinde bu nesneyi buraya atamayı unutmayın.
        /// Bu atamayı yapmamanız durumunda modüllerdeki controller ve view'lar API uygulamasında çalışmayacaklardır.
        /// Çünkü bu nesne MVC entegrasyonu sonrasındaki diğer entegrasyon adımlarında da kullanılmaktadır.
        /// </summary>
        public static IMvcBuilder MvcBuilder { get; set; }
    }
}