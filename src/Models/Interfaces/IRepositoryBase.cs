using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    public interface IRepositoryBase<TEntity> : IDisposable
        where TEntity : class, IEntityModelBase
    {
        IQueryable<TEntity> GetAll(bool asNoTracking = true);
        TEntity Find(Expression<Func<TEntity, bool>> match, bool asNoTracking = true);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, bool asNoTracking = true, CancellationToken cancellationToken = default);
        ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match, bool asNoTracking = true);
        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match, bool asNoTracking = true, CancellationToken cancellationToken = default);
        int Count(bool asNoTracking = true);
        int Count(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);
        Task<int> CountAsync(bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default);
        TEntity GetById(int id, bool asNoTracking = true);
        Task<TEntity> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default);
        ICollection<TEntity> ListByStatus(bool status = true, bool asNoTracking = true);
        Task<ICollection<TEntity>> ListByStatusAsync(bool status = true, bool asNoTracking = true, CancellationToken cancellationToken = default);
    }
}