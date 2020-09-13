using System.Threading.Tasks;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF entegrasyon esnasında modüllerden kendi event tanımlarını havuza eklemelerini ister.
    /// Bu işlemi gerçekleştirmek için her modülün kütüphanesinde yer alan ve bu interface'den kalıtım almış sınıflarını çağırır.
    /// Interface içerisinde yer alan "Start" metodu ile bu işlemi gerçekleştirir.
    /// Modül bu metot yardımıyla kendi event'larını diğer modüllerinde göreceği bir havuza ekler.
    /// </summary>
    public interface IModuleEventsAddInitializer
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Bu metot, CSBEF tarafından uygulama ilk ayağa kalkarken kullanılır.
        /// Bu durum entegrasyon aşamasıdır.
        /// CSBEF her modül içerisinde bu interface'den kalıtım almış bir sınıf arar.
        /// Bu sınıf her modülde muhakkak olmalıdır.
        /// Bulunan sınıf instance'ı alınır ve bu metot tetiklenir.
        /// Metoda IEventService interface'inden kalıtım almış ve singleton olarak oluşturulmuş bir instance gönderilir.
        /// Bu instance'ın sigleton olarak oluşturulmasının nedeni, içerisindeki event listesinin bir kere oluşmasını ve uygulama ayakta olduğu sürece tüm modüller tarafından kullanılabilmesini sağlamaktır.
        /// </summary>
        /// <param name="eventService">Tüm modüllerden elde edilen event tanımlarını ve bunlarla ilgili yardımcı metotları barındıran singleton instance</param>
        Task Start(IEventService eventService);
    }
}