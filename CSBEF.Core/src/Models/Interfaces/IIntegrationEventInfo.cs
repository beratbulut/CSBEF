using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// "IntegrationEventBus" içerisindeki handler için kullanılan delegate tanımıdır.
    /// </summary>
    /// <param name="args">Olaya abone olan metotlara gönderilecek parametre</param>
    /// <returns>ReturnModel ile result tipi bool olan model</returns>
    public delegate Task<IReturnModel<bool>> IntegrationEventDelegate(IntegrationEventArgs args);

    /// <summary>
    /// TODO: To be translated into English
    /// Entegrasyon işleminde, uygulanan her adım için bir öncesi birde sonrası şeklinde iki olay bulunmaktadır.
    /// Adımların her biri için bus içerisinde tek tek olayları tanıtmak ve yönetmek zor olacağından, EventBus içerisinde bir liste bulunmaktadır.
    /// Bu liste içerisinde bu modeller bulunur ve her biri bir olayı temsil eder. Böylece ilerleyen süre içerisinde yapılacak ek adımlarda sadece
    /// listeye ekleme yapılması yeterli olacaktır ;)
    /// </summary>
    public interface IIntegrationEventInfo
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Tanımlanan olayın event nesnesi
        /// </summary>
        event IntegrationEventDelegate EventObject;

        /// <summary>
        /// TODO: To be translated into English
        /// Tanımlanan olayın adı. Tetikleme esnasında olaylar bu isimleriyle bulunmaktadır.
        /// Bu nedenle isimlendirme standartlarına uygun bir isim tercih edilmelidir.
        /// </summary>
        string EventName { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Olayı tetikleyecek olan metottur.
        /// Olaya abone olan diğer tüm metotların asenkron çalışması gerekmektedir.
        /// Çünkü aboneleri tetiklemek için kullanılan invoke, await ile kullanılmaktadır ve metot Task tipindedir.
        /// </summary>
        /// <param name="args">Olaya abone olan metotlara gönderilecek parametre</param>
        /// <returns>Olaya abone olan metotlardan dönen ResultModel nesnesi</returns>
        Task<IReturnModel<bool>> EventHandler(IntegrationEventArgs args);
    }
}