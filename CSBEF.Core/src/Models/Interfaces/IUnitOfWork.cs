using System.Threading;
using System.Threading.Tasks;
using CSBEF.Core.Concretes;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Unit of Work Pattern için eklenmiştir.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Tek bir dbcontext kullanılarak transaction ve Unit of Work kullanılmak istendiğinde (ki böyle olması gerekir),
        /// TransactionHelper instance'ına DI'dan erişmek yerine Unit of Work üzerinden erişmek için kullanılan property.
        /// </summary>
        TransactionHelper TransactionHelper { get; }

        /// <summary>
        /// TODO: To be translated into English
        /// Henüz veri tabanına gönderilmemiş değişikliklerin gönderilmesini sağlar.
        /// Transaction desteği vardır.
        /// Yapılan değişiklikler dbcontext.save olmadan gitmeyeceğinden ve bu metodun da transaction desteği bulunduğundan, TransactionHelper üzerinden ayrıca bir
        /// transaction başlatılmasına gerek yoktur.
        /// </summary>
        /// <param name="useTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(bool useTransaction = false, CancellationToken cancellationToken = default);
    }
}