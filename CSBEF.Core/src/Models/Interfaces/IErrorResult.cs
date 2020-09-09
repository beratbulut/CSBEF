namespace CSBEF.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// IReturnModel içerisindeki hata dönüşleri için kullanılan Error bilgisi modelidir.
    /// </summary>
    public interface IErrorResult
    {
        #region Properties

        /// <summary>
        /// TODO: To be translated into English
        /// Oluşan hata ile ilgili açıklama
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Oluşan hata ile ilgili benzersiz hata tanımlama kodu.
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Hata olup olmama durumu. true: Hata var, false: Hata yok anlamındadır.
        /// İşlem sonucunda hata dönüşü olup olmadığı bu property değeri kontrol edilerek anlaşılır.
        /// </summary>
        bool Status { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Bazı hata durumları, işletilen sürecin sonlanmasını gerektirmektedir.
        /// Ancak bazı hatalar ise o anki işlem için bir önem teşkil ediyordur.
        /// Oluşan hatanın süreci ne şekilde etkileyeceği, bu bilgi ile belirlenir.
        /// Eğer değer "true" ise, bu dönüş sonrasında ilgili süreç (yani metot) sürdürülmemelidir.
        /// </summary>
        bool StopAction { get; set; }

        #endregion Properties
    }
}