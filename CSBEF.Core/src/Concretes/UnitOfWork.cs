using System.Threading;
using System.Threading.Tasks;
using CSBEF.Concretes;
using CSBEF.Core.Helpers.Exceptions;
using CSBEF.Core.Models.Interfaces;

namespace CSBEF.Core.Concretes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ModularDbContext dbContext;
        private readonly TransactionHelper transactionHelper;
        private bool disposed;

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
            Dispose(true);
        }
    }
}