using CSBEF.Models.Interfaces;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Event'ların handler metotları için kullanılacak delegate function tanımıdır.
    /// </summary>
    /// <param name="data">
    /// Her event için bir request birde response tip gerekmektedir.
    /// Bu sayede subscribe olan diğer action'lar, gerçekleşen olayla ilgili tüm bilgilere sahip olabilirler.
    /// Bu parametre, söz konusu request modele denk gelmektedir.
    /// Tipinin dynamic olmasının nedeni ise; bu tipin oluşturulan bu senaryo içerisinde asla bilinemeyecek olmasıdır.
    /// </param>
    /// <param name="eventInfo">
    /// Tetiklenecek olan event'ı tanımlayan modeldir.
    /// </param>
    /// <returns>
    /// Dönüş tipi olarak dynamic kullanılmıştır.
    /// Dönüşler için IReturnModel genel kabul edilse de, modül geliştiricileri duruma göre farklı dönüşler tercih edebilir.
    /// Bunu kısıtlamamak için dönüş tipi dynamic olarak tanımlanmıştır.
    /// </returns>
    public delegate dynamic EventDelegate(dynamic data, IEventInfo eventInfo);

    public interface IEventModel
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Event ile ilgili detayların yer aldığı info model
        /// </summary>
        /// <value></value>
        IEventInfo EventInfo { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// "EventDelegate" ile belirtilmiş Event metodu
        /// </summary>
        event EventDelegate TheEvent;

        /// <summary>
        /// TODO: To be translated into English
        /// Event'ı tetikleyecek olan handler metot.
        /// </summary>
        /// <param name="data">
        /// İlgili aksiyonun request model tipidir.
        /// </param>
        /// <typeparam name="TResult">
        /// Tüm dönüşler IReturnModel ile yapılmaktadır fakat IReturnModel generic bir modeldir.
        /// Bu sayede Result tipi dışarıdan belirtilebilir.
        /// Bu parametrede ise, kullanılacak bu Result tipi belirtilmektedir.
        /// </typeparam>
        /// <typeparam name="TParam">
        /// Request model için kullanılacak tiptir.
        /// </typeparam>
        /// <returns>
        /// "IReturnModel" tipinde dönüş yapılır. 
        /// </returns>
        IReturnModel<TResult> EventHandler<TResult, TParam>(TParam data);
    }
}