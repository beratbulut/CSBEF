using System.Threading;
using System.Threading.Tasks;
using CSBEF.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CSBEF.Concretes
{
    public class TransactionHelper : ITransactionHelper
    {
        private ModularDbContext context;
        private IDbContextTransaction Transaction { get; set; }

        public TransactionHelper(ModularDbContext context)
        {
            this.context = context;
        }

        public async Task CreateTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (context.Database.CurrentTransaction != null)
            {
                await WaitDbTransaction(cancellationToken).ConfigureAwait(false);
            }

            Transaction = await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            await Transaction.DisposeAsync().ConfigureAwait(false);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await Transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }

        public void ChangeDbContext(ModularDbContext context)
        {
            this.context = context;
        }

        private Task WaitDbTransaction(CancellationToken cancellationToken = default)
        {
            while (context.Database.CurrentTransaction != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                Thread.Sleep(500);
            }

            return Task.CompletedTask;
        }
    }
}