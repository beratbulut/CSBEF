using CSBEF.Models.Interfaces;

namespace CSBEF.Models
{
    public class RepositoryCudReturnModel<T> : IRepositoryCudReturnModel<T>
    {
        public T Entity { get; set; }

        public int EffectedRows { get; set; }
    }
}