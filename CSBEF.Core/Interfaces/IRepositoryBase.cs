using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CSBEF.Core.Interfaces {
    public interface IRepositoryBase<TEntity> : IDisposable
    where TEntity : class, IEntityModelBase {
        IQueryable<TEntity> GetAll ();
        TEntity Add (TEntity t);
        TEntity Find (Expression<Func<TEntity, bool>> match);
        ICollection<TEntity> FindAll (Expression<Func<TEntity, bool>> match);
        void Delete (TEntity entity);
        TEntity Update (TEntity t);
        int Count ();
        int Count (Expression<Func<TEntity, bool>> predicate);
        void Save ();
        IQueryable<TEntity> GetAllIncluding (params Expression<Func<TEntity, object>>[] includeProperties);
    }
}