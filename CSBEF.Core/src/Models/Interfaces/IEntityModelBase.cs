using System;

namespace CSBEF.src.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Entity Modeller için kullanılan interface'dir.
    /// Bu interface'den kalıtım alan modeller, poco model olarak değerlendirilmektedir.
    /// </summary>
    public interface IEntityModelBase
    {
        #region Properties

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili kaydın PrimaryKey bilgisidir.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili kaydın kullanım durumudur. Eğer ilgili kayıt silinmişse bu property'nin değeri "false" olmaktadır.
        /// Bu nedenle tablolarda sorgulama yaparken bu bilgi kontrol edilmelidir.
        /// </summary>
        bool Status { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili kaydın eklenme zamanı bilgisidir.
        /// </summary>
        DateTime AddingDate { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili kaydın son güncelleme zamanıdır. Bir kayıt ilk eklendiğinde bu alana eklenme zamanı set edilmektedir.
        /// Çünkü kayıt eklenirken aynı zamanda da son güncellemesi yapılmış olarak sayılmaktadır.
        /// </summary>
        DateTime UpdatingDate { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili kaydı ekleyen kullanıcının kayıt numarası.
        /// 
        /// Bu bilginin herhangi bir tabloyla bağlantısı yoktur. Çünkü CSBEF içerisinde entegre gelen bir kullanıcı denetimi yapısı yoktur.
        /// Fakat muhakkak bir kullanıcı denetimi modülü ilgili API uygulamasında yer alacaktır. Sonuç olarak, böyle bir alan verilerde gerekebileceği için eklendi.
        /// Eğer herhangi bir kullanıcı denetimi kullanılmıyorsa, bu alana varsayılan olarak 1 atanmalıdır. Bu alan asla null bırakılmamalıdır.
        /// </summary>
        int AddingUserId { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// İlgili kaydı son güncelleyen kullanıcının kayıt numarası.
        /// 
        /// Bu bilginin herhangi bir tabloyla bağlantısı yoktur. Çünkü CSBEF içerisinde entegre gelen bir kullanıcı denetimi yapısı yoktur.
        /// Fakat muhakkak bir kullanıcı denetimi modülü ilgili API uygulamasında yer alacaktır. Sonuç olarak, böyle bir alan verilerde gerekebileceği için eklendi.
        /// Eğer herhangi bir kullanıcı denetimi kullanılmıyorsa, bu alana varsayılan olarak 1 atanmalıdır. Bu alan asla null bırakılmamalıdır.
        /// </summary>
        int UpdatingUserId { get; set; }

        #endregion Properties
    }
}