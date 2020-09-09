using System;
using System.Collections.Generic;
using CSBEF.Models.Interfaces;

namespace CSBEF.Helpers
{
    /// <summary>
    /// TODO: To be translated into English
    /// Genel olarak tüm dönüşlerde "IReturnModel" tipi kullanılmaktadır.
    /// Bu model içerisinde ErrorInfo yer almaktadır ve bu modelin de içerisinde "StopAction" adında bir property yer almaktadır.
    /// Kodlama içerisinde önce hata kontrolü, ardından da "StopAction" kontrolü yapılmalıdır. Eğer "StopAction" true gelmişse, throw fırlatılmalıdır.
    /// Bu sınıf, bu işlemin tek bir satırla yapılmasını ve if kullanılmamasını sağlamaktadır.
    /// 
    /// Bu helper yokken normal şartlarda "StopAction" kullanımı şu şekildedir;
    /// if(returnModel.ErrorInfo.Status && returnModel.ErrorInfo.StopAction)
    ///     throw new Exception(returnModel.ErrorInfo.Message + " (" + returnModel.ErrorInfo.Code + ")");
    /// 
    /// Bu helper sayesinde bu kontrol şu şekilde yapılmaktadır;
    /// returnModel.ThrowIfErrorInfoStopAction();
    /// </summary>
    public static class ThrowIfErrorInfoStopActionValidator
    {
        public static IReturnModel<T> ThrowIfErrorInfoStopAction<T>([ValidatedErrorInfoStopAction] this IReturnModel<T> value)
        {
            if (EqualityComparer<IReturnModel<T>>.Default.Equals(value, default))
            {
                value.ThrowIfNull();
                throw new Exception(value.ErrorInfo.Message + " (" + value.ErrorInfo.Code + ")");
            }

            return value;
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedErrorInfoStopActionAttribute : Attribute
        {

        }
    }
}