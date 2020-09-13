using CSBEF.Core.Models.Interfaces;
using CSBEF.Models.Interfaces;

namespace CSBEF.Core.Models
{
    public class EventModel : IEventModel
    {
        public IEventInfo EventInfo { get; set; }

        public event EventDelegate TheEvent;

        public EventModel()
        {
            EventInfo = new EventInfo();
        }

        public IReturnModel<TResult> EventHandler<TResult, TParam>(TParam data)
        {
            // TODO: To be translated into English
            /*
            Bu kısımda dikkat edilmesi ve kafa karıştırıcı gibi görünen önemli bir nokta bulunmakta;
            Event tanımında kullanılan delegate'in giriş ve çıkış tipleri dynamic şeklindedir.
            Fakat bu handler metoda bakıldığında dönüş tipi ön tanımlı olarak görülmektedir (yani IReturnModel).
            Öncelikle delegate kısmında dynamic kullanma amacımız, delegate tanımında, bu kısımda gerekli olan TResult ile TParam T tanımlarını belirtemiyor olmamızdır.
            Girdi kısmının da ne olacağını bilemediğimizden bu kısımda da dynamic kullandık.
            Öncelikle bu kısım bir kafa karışıklığına yol açabileceği için ayrıca açıklama gereği hissettim.

            Bunun yanı sıra,
            Modüllerde bu handler metotlar tetiklendikten sonra dönüş değerinin null olup olmadığı kontrol edilmelidir.
            Bunun sebebi; eğer söz konusu event'ın herhangi bir subscribe'ı yoksa, herhangi bir manipülasyonda yok demektir.
            Bu nedenle değer null dönecektir. Belki planlanan sürece göre herhangi bir müdahalenin olup olmadığı, geliştirici için önem teşkil ediyor olabilir.
            Eğer dönüş null ise, subscribe yok anlamına geliyor.
            Eğer dönüş null değilse ise, subscribe var anlamına geliyor ve muhakkak bu tarz durumlarda IReturnModel.ErrorInfo.Status değerine bakılmalıdır.
            Çünkü subscribe metotlar, özellikle before event'larda süreci durdurmak istiyorlarsa, bu değeri true olarak set ediyorlar.
            */

            if (TheEvent != null)
            {
                return (IReturnModel<TResult>)TheEvent.Invoke(data, EventInfo);
            }
            else
            {
                return null;
            }
        }
    }
}