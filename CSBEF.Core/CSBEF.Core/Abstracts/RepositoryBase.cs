using AutoMapper;
using CSBEF.Core.Concretes;
using CSBEF.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSBEF.Core.Abstracts
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, IEntityModelBase
    {
        #region Dependencies

        protected ModularDbContext _context;
        private IMapper _mapper;

        #endregion Dependencies

        #region Construction

        public RepositoryBase(ModularDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #endregion Construction

        #region Actions

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public virtual async Task<ICollection<TEntity>> GetAllAsyn()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual TEntity Add(TEntity t)
        {
            var entity = _context.Set<TEntity>().Add(t).Entity;
            return entity;
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return GetAll().FirstOrDefault(match);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await GetAll().FirstOrDefaultAsync(match);
        }

        public virtual ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return GetAll().Where(match).ToList();
        }

        public virtual async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await GetAll().Where(match).ToListAsync();
        }

        public virtual void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual TEntity Update(TEntity t)
        {
            _context.Entry(t).State = EntityState.Modified;

            return t;
        }

        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public virtual async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).CountAsync();
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public virtual async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = GetAll();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return queryable;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Actions
    }
}