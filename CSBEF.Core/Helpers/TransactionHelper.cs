using CSBEF.Core.Concretes;
using CSBEF.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace CSBEF.Core.Helpers
{
    public class TransactionHelper : ITransactionHelper
    {
        private readonly ModularDbContext _context;
        private IDbContextTransaction Transaction { get; set; }

        public TransactionHelper(ModularDbContext context)
        {
            _context = context;
        }

        public async Task CreateTransaction()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await Task.Run(() =>
                {
                    WaitDbTransaction();
                }).ConfigureAwait(false);
            }

            Transaction = _context.Database.BeginTransaction();
        }

        public void EndTransaction()
        {
            Transaction.Commit();
            Transaction.Dispose();
        }

        public void CancelTransaction()
        {
            Transaction.Rollback();
        }

        private void WaitDbTransaction()
        {
            while (_context.Database.CurrentTransaction != null)
            {
                Thread.Sleep(500);
            }

            _ = Task.CompletedTask;
        }
    }
}