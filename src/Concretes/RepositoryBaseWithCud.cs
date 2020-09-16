using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSBEF.enums;
using CSBEF.Helpers;
using CSBEF.Models.Interfaces;

namespace CSBEF.Concretes
{
    public class RepositoryBaseWithCud<T> : RepositoryBase<T>, IRepositoryBaseWithCud<T>
        where T : class, IEntityModelBase
    {
        private readonly ModularDbContext dbContext;
        private readonly bool inWorker;

        public RepositoryBaseWithCud(ModularDbContext dbContext, bool inWorker = false) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.inWorker = inWorker;
        }

        public virtual int Save()
        {
            int effected = 0;

            if (!inWorker)
                effected = dbContext.SaveChanges();

            return effected;
        }

        public virtual async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            int effected = 0;

            if (!inWorker)
                effected = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return effected;
        }

        public virtual int Add(T entity, bool useSave = true)
        {
            dbContext.Set<T>().Add(entity);
            return inWorker ? 0 : useSave ? Save() : 0;
        }

        public virtual async Task<int> AddAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            return inWorker ? 0 : useSave ? await SaveAsync(cancellationToken).ConfigureAwait(false) : 0;
        }

        public virtual int AddRange(IEnumerable<T> entities, bool useSave = true)
        {
            dbContext.Set<T>().AddRange(entities);
            return inWorker ? 0 : useSave ? Save() : 0;
        }

        public virtual async Task<int> AddRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<T>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            return inWorker ? 0 : useSave ? await SaveAsync(cancellationToken).ConfigureAwait(false) : 0;
        }

        public virtual int Update(T entity, bool useSave = true)
        {
            var state = dbContext.GetEntryState(entity);
            if (state != ContextEntityStates.Added)
            {
                dbContext.Update(entity);
            }

            return inWorker ? 0 : useSave ? Save() : 0;
        }

        public virtual async Task<int> UpdateAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => { return Update(entity, useSave); }, cancellationToken).ConfigureAwait(false);
        }

        public virtual int UpdateRange(IEnumerable<T> entities, bool useSave = true)
        {
            entities.ThrowIfNull();

            int effectedRows = 0;

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    effectedRows += Update(entity, useSave);
                }
            }

            return effectedRows;
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default)
        {
            entities.ThrowIfNull();

            int effectedRows = 0;

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    effectedRows += await UpdateAsync(entity, useSave, cancellationToken).ConfigureAwait(false);
                }
            }

            return effectedRows;
        }

        public virtual int SoftDelete(T entity, bool useSave = true)
        {
            entity.ThrowIfNull();

            entity.Status = false;

            return Update(entity, useSave);
        }

        public virtual async Task<int> SoftDeleteAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default)
        {
            entity.ThrowIfNull();

            entity.Status = false;

            return await UpdateAsync(entity, useSave, cancellationToken).ConfigureAwait(false);
        }

        public virtual int SoftDeleteRange(IEnumerable<T> entities, bool useSave = true)
        {
            entities.ThrowIfNull();

            int effectedRows = 0;

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    effectedRows += SoftDelete(entity, useSave);
                }
            }

            return effectedRows;
        }

        public virtual async Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default)
        {
            entities.ThrowIfNull();

            int effectedRows = 0;

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    effectedRows += await SoftDeleteAsync(entity, useSave, cancellationToken).ConfigureAwait(false);
                }
            }

            return effectedRows;
        }

        public virtual int HardDelete(T entity, bool useSave = true)
        {
            dbContext.Remove(entity);
            return inWorker ? 0 : Save();
        }

        public virtual async Task<int> HardDeleteAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { dbContext.Remove(entity); }, cancellationToken).ConfigureAwait(false);
            return inWorker ? 0 : await SaveAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual int HardDeleteRange(IEnumerable<T> entities, bool useSave = true)
        {
            entities.ThrowIfNull();

            int effectedRows = 0;

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    effectedRows += HardDelete(entity, useSave);
                }
            }

            return effectedRows;
        }

        public virtual async Task<int> HardDeleteRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default)
        {
            entities.ThrowIfNull();

            int effectedRows = 0;

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    effectedRows += await HardDeleteAsync(entity, useSave, cancellationToken).ConfigureAwait(false);
                }
            }

            return effectedRows;
        }
    }
}