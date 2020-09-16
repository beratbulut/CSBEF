using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CSBEF.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Concretes
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, IEntityModelBase
    {
        private ModularDbContext dbContext;

        private bool disposed;

        public RepositoryBase(ModularDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void ChangeDbContext(ModularDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public virtual IQueryable<TEntity> GetAll(bool asNoTracking = true)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();
            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> match, bool asNoTracking = true)
        {
            return GetAll(asNoTracking).FirstOrDefault(match);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await GetAll(asNoTracking).Where(match).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match, bool asNoTracking = true)
        {
            return GetAll(asNoTracking).Where(match).ToList();
        }

        public virtual async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await GetAll(asNoTracking).Where(match).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual int Count(bool asNoTracking = true)
        {
            return GetAll(asNoTracking).Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true)
        {
            return GetAll(asNoTracking).Where(predicate).Count();
        }

        public virtual async Task<int> CountAsync(bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await GetAll(asNoTracking).CountAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await GetAll(asNoTracking).Where(predicate).CountAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetById(Guid id, bool asNoTracking = true)
        {
            return GetAll(asNoTracking).Where(i => i.Id == id).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await GetAll(asNoTracking).Where(i => i.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual ICollection<TEntity> ListByStatus(bool status = true, bool asNoTracking = true)
        {
            return GetAll(asNoTracking).Where(i => i.Status == status).ToList();
        }

        public virtual async Task<ICollection<TEntity>> ListByStatusAsync(bool status = true, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await GetAll(asNoTracking).Where(i => i.Status == status).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}