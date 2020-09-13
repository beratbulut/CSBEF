using System.Threading;
using System.Threading.Tasks;
using CSBEF.Concretes;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Veri tabanı işlemlerinin bir transaction içerisinde yapılması istendiğinde kullanılacak helper sınıfıdır.
    /// </summary>
    public interface ITransactionHelper
    {
        /// <summary>
        /// TODO: To be translated into English
        /// DbContext üzerinde yeni bir transaction başlatır.
        /// Eğer hali hazırda devam eden bir transaction varsa, bu bitene kadar metodu çağıran yeri bekletir.
        /// </summary>
        Task CreateTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// TODO: To be translated into English
        /// Güncel transaction işlemini commit biçiminde sonlandırır.
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// TODO: To be translated into English
        /// Güncel transaction işlemini iptal eder, rollback yapar.
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// TODO: To be translated into English
        /// Unit of Work içerisinde kullanılacağı zaman ve DbContext Lifetime "Transient" ise, Unit of Work ile bu sınıf içerisindeki dbcontext instance'ı aynı olmalıdır.
        /// Bu instance alındığında DI'dan DbContext alınmaktadır. Fakat bu eşitleme ihtiyacı nedeniyle bu metot geliştirilmiştir. Metot parametre ile aldığı DbContext'i
        /// kendi içindekiyle değiştirir.
        /// </summary>
        /// <param name="context">Kullanılacak DbContext instance'ı</param>
        void ChangeDbContext(ModularDbContext context);
    }
}