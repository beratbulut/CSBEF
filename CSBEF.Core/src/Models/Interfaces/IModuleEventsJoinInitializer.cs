using System.Threading.Tasks;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF entegrasyon aşamasında tüm modüllerin "IModuleEventsAddInitializer" interface'inden kalıtım almış sınıflarından kendi event tanımlarını alır ve bunlardan bir liste oluşturur.
    /// Daha sonrasında tüm modüllere bu listeyi göndererek istedikleri event'lara kendi metotlarını bağlamalarını ister.
    /// "IModuleEventsAddInitializer" ile modüllerin listeye ekledikleri event'lara bu interface'den kalıtım almış sınıflarıyla da subscribe olurlar.
    /// 
    /// Buradaki mantık şu şekilde işlemektedir;
    /// İlk önce tüm modüllerden event tanımları alınır. Çünkü bu interface ile listedeki bir event'a subscribe olmak isteyen modülün bu event tanımını muhakkak bu havuzda bulabiliyor olması gerekir.
    /// Bu nedenle ilk önce event tanımları toplanır, sonra subscibe olma aşaması başlar.
    /// Eğer bir modül, subscribe olacağı event'ı bulamazsa, ilgili modül API uygulamasında yer almıyor demektir. Buna göre bir aksiyon almalıdır.
    /// Bu aksiyona örnek; modül olmadığı için kendi de çalışmayabilir; uygulamanın bu modül olmadan çalışmasını engelleyebilir, ilgili aksiyonları yapmadan işlemlerini gerçekleştirebilir.
    /// 
    /// Örnek;
    /// Bir modülün bir işlem sonucunda göndereceği bilgileri manipüle ederek değiştirmek isteyen bir başka modül olabilir.
    /// Eğer bu ikinci modülün üretilme amacı sadece bu iş ise, bu modülün API uygulamasında muhakkak olması gerekir.
    /// Aksi durumda ikinci modül hiç iş yapmayacaktır ve bu nedenle çalışması gerekmez.
    /// Eğer ikinci modülün çalışmaması sistemi engelleyen bir faktörse, ikinci modül ihtiyacı olan event tanımlarını bulamadığında uygulamanın durmasını sağlamak için entegrasyonun bu aşamasında hata döndürmelidir.
    /// CSBEF entegrasyon süreci, EventJoin aşamasında herhangi bir modülün hata döndürmesi durumunda entegrasyon sürecini kesecek ve uygulamanın hata vererek durmasını sağlayacaktır.
    /// Eğer ikinci modülün çalışamayacak olması sistemin genelini etkileyen bir durum değilse, bu aşamada ikinci modül hata döndürmez ancak gereksiz olarak sistemde yer alır, işlem yapmaz.
    /// Eğer ikinci modülün tek işi bu değilse, sadece bu işini yapamamış olur.
    /// Bu senaryoda, birinci modül ana modül olurken, ikinci modül de alt modül olmuş olur. Yada birinci modül ile ikinci modül arasında ilişki var demektir.
    /// Bir modül geliştiren kişi, kendi modülüyle ilişkisi olan alt modülleri bilmeyebilir. Çünkü birinci modülü geliştiren kişi ikinci modülü geliştiren kişiyi bilmeyebilir.
    /// Bu nedenle bir event'a subscribe olunduğunda, ilerleyen versiyonlarda süreç değiştirilirse, önceki sürece göre yazılmış alt modüllerin var olabileceği düşünülerek tedbir alınmalıdır.
    /// Aslında bu nedenle her bir modülü bir ürün olarak değerlendirmek gerekir.
    /// </summary>
    public interface IModuleEventsJoinInitializer
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Modül bu metot tetiklendiğinde kendi subscribe sürecinin başladığını anlar.
        /// Kendisine gelen EventService instance'ı üzerinden ihtiyacı olan event tanımlarını bulur ve subscribe olur.
        /// Her modülde bu instance muhakkak olmalıdır.
        /// Eğer subscribe olacağı herhangi bir event yoksa bile bu instance yer almalı fakat metodun içerisinde herhangi bir işlem yapılmadan direk return edilmelidir.
        /// </summary>
        /// <param name="eventService">Tüm modüllerden elde edilen event tanımlarını ve bunlarla ilgili yardımcı metotları barındıran singleton instance</param>
        Task Start(IEventService eventService);
    }
}