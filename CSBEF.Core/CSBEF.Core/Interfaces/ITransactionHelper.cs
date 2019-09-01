using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public interface ITransactionHelper
    {
        Task CreateTransaction();

        void EndTransaction();

        void CancelTransaction();
    }
}