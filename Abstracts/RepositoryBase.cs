using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSBEF.Core.Concretes;
using CSBEF.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Core.Abstracts {
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, IEntityModelBase {
            #region Construction

            public RepositoryBase (ModularDbContext context) {
                DBContext = context;
            }

            #endregion Construction

            #region Actions

            public ModularDbContext DBContext { get; }

            public virtual IQueryable<TEntity> GetAll () {
                return DBContext.Set<TEntity> ().AsNoTracking ();
            }
            public virtual TEntity Add (TEntity t) {
                var entity = DBContext.Set<TEntity> ().Add (t).Entity;
                return entity;
            }
            public virtual TEntity Find (Expression<Func<TEntity, bool>> match) {
                return GetAll ().FirstOrDefault (match);
            }
            public virtual ICollection<TEntity> FindAll (Expression<Func<TEntity, bool>> match) {
                return GetAll ().Where (match).ToList ();
            }
            public virtual void Delete (TEntity entity) {
                DBContext.Set<TEntity> ().Remove (entity);
            }
            public virtual TEntity Update (TEntity t) {
                DBContext.Entry (t).State = EntityState.Modified;

                return t;
            }
            public virtual int Count () {
                return GetAll ().Count ();
            }
            public virtual int Count (Expression<Func<TEntity, bool>> predicate) {
                return GetAll ().Where (predicate).Count ();
            }
            public virtual void Save () {
                DBContext.SaveChanges ();
            }
            public virtual IQueryable<TEntity> GetAllIncluding (params Expression<Func<TEntity, object>>[] includeProperties) {
                IQueryable<TEntity> queryable = GetAll ();
                foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties) {
                    queryable = queryable.Include (includeProperty);
                }

                return queryable;
            }

            private bool disposed = false;
            protected virtual void Dispose (bool disposing) {
                if (!disposed) {
                    if (disposing) {
                        DBContext.Dispose ();
                    }
                    disposed = true;
                }
            }
            public void Dispose () {
                Dispose (true);
                GC.SuppressFinalize (this);
            }

            #endregion Actions
        }
}