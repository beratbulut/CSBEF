using CSBEF.Core.enums;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Modüller arasındaki haberleşme event'lar aracılığıyla sağlanır.
    /// Modül yapısında alt ve üst modül kavramları yer almaktadır.
    /// Bir modülün üst yada alt modül olduğu, herhangi bir event'a subscribe olup olmamasıyla anlaşılır.
    /// Her modül, CSBEF entegrasyon aşamasında kendi event tanımlarını belirtir. Bu olaylar bir havuzda toplanır.
    /// Daha sonrasında yine kontrollü bir şekilde tüm modüllere bu havuz sunularak, isteyen modülün istediği event'a subscribe olması sağlanır.
    /// Eğer bir modül bu aşamada herhangi bir başka modülün event'ına subscribe olursa, bu event'ın sahibi olan diğer modülün alt modülü olmuş anlamına gelir.
    /// Bir diğer bakış açısıyla da buna modüller arası ilişki de denebilir.
    /// Bu interface, söz konusu modül event'larını temsil etmektedir ve çeşitli bilgiler barındırır.
    /// Bu bilgiler sayesinde modül geliştiren developer ve aynı zamanda ilgili event'ı tetikleyecek yada ele alacak diğer mekanizmalar, yolunu daha rahat bulur.
    /// </summary>
    public interface IEventInfo
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Event'ın adı. Değişken isimlendirme standartına uyulmalıdır.
        /// </summary>
        string EventName { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili modülün tam adı.
        /// Bu sayede hangi event'ın hangi modüle ait olduğu anlaşılır.
        /// Örnek kullanım; "CSBEF.Module.UserManagement" şeklinde modülün tam adı belirtilmelidir.
        /// </summary>
        string ModuleName { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili event tetiklendiğinde kullanılacak action'ı barındıran servisin adıdır.
        /// Class adı kullanılmalıdır.
        /// Örnek kullanım: "UserService".
        /// Bu belirtilen servis adı, "ModuleName" ile modül havuzundan elde edilen kütüphane içerisinde aranarak ilgili instance'a ServiceProvider ile erişmek için kullanılır.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Event tetiklendiğinde çalıştırılacak action adıdır.
        /// "ServiceName" ile belirtilen class'ın içerisinde yer alan public bir metot adı olmalıdır.
        /// </summary>
        string ActionName { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Event'ın türünü belirtir.
        /// Türler event'ların tetiklenme zamanını belirtir.
        /// </summary>
        EventTypeEnum EventType { get; set; }
    }
}