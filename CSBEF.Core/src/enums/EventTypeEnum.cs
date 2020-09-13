namespace CSBEF.Core.enums
{
    /// <summary>
    /// TODO: To be translated into English
    /// Modüller kendi aralarında event'lar üzerinden haberleşmektedirler.
    /// Ayrıca CSBEF.Core içerisinde de kısıtlı sayıda main event bulunmaktadır.
    /// Modüller bu main event'lara da dahil olarak sürece müdahale edebilmektedirler.
    /// Bu enum, tanımlanan bir event'ın tipini belirtmektedir.
    /// Tanımlanan tüm bu event'lar, before ve after şeklinde tanımlanmaktadır.
    /// Bu sayade ilgili event'ın bir işlemin öncesinde veya sonrasında tetiklendiği anlaşılır.
    /// Çünkü, bir event'ın tetiklenme zamanına göre aksiyonu farklıdır.
    /// Before event'lar, gerçekleşecek bir olayı manipüle edebilirler, gelen datayı değiştirebilirler veya olayın temsil ettiği aksiyonu devre dışı bırakabilirler.
    /// After event'lar ise sadece dönülecek olan result'ı değiştirebilirler ve ilgili action'a herhangi bir manipülasyonları olmaz.
    /// Çünkü ilgili action çalışmış ve işini yapmıştır.
    /// </summary>
    public enum EventTypeEnum
    {
        Before,
        After
    }
}