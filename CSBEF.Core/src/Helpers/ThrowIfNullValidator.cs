using System;
using System.Collections.Generic;

namespace CSBEF.Helpers
{
    /// <summary>
    /// TODO: To be translated into English
    /// Metotların parametrelerini kontrol etmek için kullanılan bir doğrulayıcı yardımcı sınıfıdır.
    /// </summary>
    public static class ThrowIfNullValidator
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Metotların başlangıçlarında, gelen parametrelerin null olup olmadıklarını kontrol eder.
        /// Bu kontrol, null beklenmeyen parametreleri kontrol etmek için kullanılır.
        /// Kontrol gerçekleştirilen parametre null değerindeyse, "ArgumentNullException" tipinde Exception fırlatılır.
        /// Örnek kullanım:
        ///     parametrem.ThrowIfNull(nameof(parametrem));
        /// </summary>
        /// <param name="value">
        /// Üzerinde kontrol gerçekleştirilecek parametre değişkeni
        /// </param>
        /// <param name="name">
        /// Üzerinde kontrol gerçekleştirilecek ve eğer gerekiyorsa fırlatılacak olan Exception için kullanılacak parametre adı (çoğu zaman "nameof(parametrem)" olarak gönderilir)
        /// Eğer değer gönderilmezse bu parametre için "nameof(T)" değeri atanır (yani kontrol edilen parametrenin adı yerleştirilir).
        /// </param>
        /// <typeparam name="T">
        /// Üzerinde kontrol gerçekleştirilen parametrenin tipi.
        /// </typeparam>
        /// <returns>
        /// Eğer doğrulama başarısız olmuşsa, throw ile exception fırlatılmış demektir ve dönüş olmaz.
        /// Eğer hata yoksa, üzerinde kontrol gerçekleştirilen parametre dönülür.
        /// </returns>
        public static T ThrowIfNull<T>([ValidatedNotNull] this T value, string name = nameof(T))
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedNotNullAttribute : Attribute
        {

        }
    }
}