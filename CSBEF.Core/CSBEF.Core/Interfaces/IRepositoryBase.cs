using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public interface IRepositoryBase<TEntity> : IDisposable
        where TEntity : class, IEntityModelBase
    {
        IQueryable<TEntity> GetAll();

        Task<ICollection<TEntity>> GetAllAsyn();

        TEntity Add(TEntity t);

        TEntity Find(Expression<Func<TEntity, bool>> match);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);

        ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match);

        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);

        void Delete(TEntity entity);

        TEntity Update(TEntity t);

        int Count();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        void Save();

        Task<int> SaveAsync();

        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        Task<ICollection<TEntity>> FindByAsyn(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
    }
}