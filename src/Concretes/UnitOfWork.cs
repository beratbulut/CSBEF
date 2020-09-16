using System;
using System.Threading;
using System.Threading.Tasks;
using CSBEF.Helpers;
using CSBEF.Helpers.Exceptions;
using CSBEF.Interfaces;

namespace CSBEF.Concretes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ModularDbContext dbContext;
        private readonly TransactionHelper transactionHelper;

        public TransactionHelper TransactionHelper
        {
            get
            {
                return transactionHelper;
            }
        }

        public UnitOfWork(ModularDbContext dbContext, TransactionHelper transactionHelper)
        {
            this.dbContext = dbContext;
            this.transactionHelper = transactionHelper;

            this.transactionHelper.ChangeDbContext(this.dbContext);
        }

        public IRepositoryBase<T> GenerateRepository<T>()
            where T : class, IEntityModelBase
        {
            return new RepositoryBase<T>(dbContext);
        }

        public IRepositoryBaseWithCud<T> GenerateRepositoryWithCud<T>()
            where T : class, IEntityModelBase
        {
            return new RepositoryBaseWithCud<T>(dbContext);
        }

        public async Task<int> SaveChangesAsync(bool useTransaction = false, CancellationToken cancellationToken = default)
        {
            int result;

            if (useTransaction)
            {
                await transactionHelper.CreateTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    await transactionHelper.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (UnitOfWorkSaveChangesException ex)
                {
                    await transactionHelper.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    return await Task.FromException<int>(ex).ConfigureAwait(false);
                }
            }
            else
            {
                try
                {
                    result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (UnitOfWorkSaveChangesException ex)
                {
                    return await Task.FromException<int>(ex).ConfigureAwait(false);
                }
            }

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}