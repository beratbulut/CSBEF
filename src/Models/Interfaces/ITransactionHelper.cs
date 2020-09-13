using System.Threading;
using System.Threading.Tasks;
using CSBEF.Concretes;

namespace CSBEF.Models.Interfaces
{
    public interface ITransactionHelper
    {
        Task CreateTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);

        void ChangeDbContext(ModularDbContext context);
    }
}