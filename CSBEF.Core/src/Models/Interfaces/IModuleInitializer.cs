using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// CSBEF modüler bir yapı olduğundan, birçok tanımlama da yine modüler olarak yapılmaktadır.
    /// Bu interface ise, modüllerde en az 1 sınıfta kullanılıyor olması gerekir.
    /// Modülde bulunan bu sınıf, CSBEF entegrasyon aşamasında tetiklenmektedir.
    /// Böylece modül kendi tanımlarını ServiceProvider içerisine yapabilmektedir.
    /// Bunlara örnek olarak servisler ve repository'ler verilebilir.
    /// </summary>
    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection);
    }
}