using System;

namespace CSBEF.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF ve modüllerinin içerisindeki tüm işleyişlerde kullanılan global dönüş modelidir.
    /// Bu model aynı zamanda modüllerin Controller.Action dönüşlerinde de genel olarak kullanılır.
    /// </summary>
    /// <typeparam name="TResult">Result property'si için kullanılacak type</typeparam>
    public interface IReturnModel<TResult>
    {
        #region Properties

        /// <summary>
        /// TODO: To be translated into English
        /// Eğer hata dönüşü yapılacaksa bu property kullanılmaktadır.
        /// </summary>
        IErrorResult ErrorInfo { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İşlem sonucunda dönülen sonuç (belirtilen TResult tipinde)
        /// </summary>
        TResult Result { get; set; }

        #endregion

        #region Public Actions

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipi enum ile dönüş yapmak için kullanılır.
        /// </summary>
        IReturnModel<TResult> SendError<T>(T errorInfo, bool stopAction = false) where T : struct, IConvertible;

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde hem enum hem de oluşan exception nesnesi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError<T>(T errorInfo, Exception ex, bool stopAction = false) where T : struct, IConvertible;

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde hem enum hem oluşan exception nesnesi hem de result değeri gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError<T>(T errorInfo, Exception ex, TResult result, bool stopAction = false) where T : struct, IConvertible;

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde sadece oluşan hata ile ilgili string bilgi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError(string message, bool stopAction = false);

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde, oluşan hata ile ilgili açıklama ve oluşan exception nesnesi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError(string message, Exception ex, bool stopAction = false);

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde, oluşan hata ile ilgili açıklama, oluşan exception nesnesi ve result bilgisi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError(string message, Exception ex, TResult result, bool stopAction = false);

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde, oluşan hata ile ilgili açıklama ve oluşan hatayla ilgili hata kodu gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError(string message, string code, bool stopAction = false);

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde, oluşan hata ile ilgili açıklama, hata kodu ve oluşan exception nesnesi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError(string message, string code, Exception ex, bool stopAction = false);

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde, oluşan hata ile ilgili açıklama, hata kodu, oluşan exception nesnesi ve result bilgisi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendError(string message, string code, Exception ex, TResult result, bool stopAction = false);

        /// <summary>
        /// TODO: To be translated into English
        /// Hızlı dönüş yapmayı sağlayan yardımcı metot.
        /// Bu kullanım tipinde sadece result bilgisi gönderilir.
        /// </summary>
        IReturnModel<TResult> SendResult(TResult result);

        #endregion
    }
}