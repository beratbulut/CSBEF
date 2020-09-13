using System.Collections.Generic;
using CSBEF.Core.enums;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF tarafından modüllerden toplanılan event tanımlarından oluşan havuzda işlem yapabilmek için kullanılan servistir.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Event listesindeki bir event'a erişmek için kullanılan metottur.
        /// </summary>
        /// <param name="moduleName">Erişilmek istenen event tanımının yer aldığı modülün adı. Örn: "CSBEF.Module.UserManagement"</param>
        /// <param name="eventName">Erişilmek istenen event'ın adı. Örn: "CheckTokenBefore"</param>
        /// <returns>Eğer event bulunursa "IEventModel" tipinde nesne, bulunamazsa boş model döner</returns>
        IEventModel GetEvent(string moduleName, string eventName);

        /// <summary>
        /// TODO: To be translated into English
        /// Event listesine yeni bir event tanımı eklemek için kullanılan metottur.
        /// </summary>
        /// <param name="eventName">Eklenecek olan event'ın adı. Örn: "CheckTokenBefore"</param>
        /// <param name="moduleName">Eklenecek olan event'ın sahibi olan modülün adı. Örn: "CSBEF.Module.UserManagement"</param>
        /// <param name="serviceName">Eklenecek olan event'ın temsil ettiği servisin adı. Örn: "TokenService"</param>
        /// <param name="actionName">Eklenecek olan event'ın temsil ettiği işlemin gerçekleştiği action adı. Örn: "CheckToken"</param>
        /// <param name="eventType">Eklenecek olan event'ın çalışma zamanını belirten event tipi. Eğer event ilgili action içerisindeki işlemlerden önce çalışıyorsa "Before", işlemler gerçekleştirilip return edilmeden önce çalışacaksa "After" tipinde olmalıdır.</param>
        void AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType);

        /// <summary>
        /// TODO: To be translated into English
        /// Tüm modüllerden elde edilmiş event tanımlarının listesine erişmek için kullanılan metottur.
        /// </summary>
        /// <returns>Event tanımlarından oluşan liste. Null gelme olasılığı yoktur, boş olabilir.</returns>
        List<IEventModel> GetAllEvents();
    }
}