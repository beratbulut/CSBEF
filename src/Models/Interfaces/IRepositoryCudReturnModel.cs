namespace CSBEF.Models.Interfaces
{
    public interface IRepositoryCudReturnModel<T>
    {
        T Entity { get; set; }

        int EffectedRows { get; set; }
    }
}