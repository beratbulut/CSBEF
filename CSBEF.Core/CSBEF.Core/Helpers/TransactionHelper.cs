using CSBEF.Core.Concretes;
using CSBEF.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace CSBEF.Core.Helpers
{
    public class TransactionHelper : ITransactionHelper
    {
        private ModularDbContext _context;
        private IDbContextTransaction _transaction { get; set; }

        public TransactionHelper(ModularDbContext context)
        {
            _context = context;
        }

        public async Task CreateTransaction()
        {
            if(_context.Database.CurrentTransaction != null)
            {
                await Task.Run(() =>
                {
                    WaitDbTransaction();
                });
            }

            _transaction = _context.Database.BeginTransaction();
        }

        public void EndTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
        }

        public void CancelTransaction()
        {
            _transaction.Rollback();
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
