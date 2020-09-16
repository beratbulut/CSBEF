using System;
using System.Threading;
using System.Threading.Tasks;
using CSBEF.Concretes;

namespace CSBEF.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        TransactionHelper TransactionHelper { get; }
        Task<int> SaveChangesAsync(bool useTransaction = false, CancellationToken cancellationToken = default);
        IRepositoryBase<T> GenerateRepository<T>() where T : class, IEntityModelBase;
        IRepositoryBaseWithCud<T> GenerateRepositoryWithCud<T>() where T : class, IEntityModelBase;
    }
}