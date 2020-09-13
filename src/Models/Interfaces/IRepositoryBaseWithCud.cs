namespace CSBEF.Models.Interfaces
{
    public interface IRepositoryBaseWithCud<T> : IRepositoryBase<T>
        where T : class, IEntityModelBase
    {

    }
}