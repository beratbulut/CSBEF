using System;
using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    public interface IServiceBase : IDisposable
    {

    }

    public interface IServiceBase<TPoco, TDTO> : IModuleService, IDisposable
        where TPoco : class, IEntityModelBase, new()
        where TDTO : class, IDtoModelBase, new()
    {
        IRepositoryBase<TPoco> Repository { get; set; }

        Task<IReturnModel<TDTO>> First(GenericFilterModel<TDTO> args);

        Task<IReturnModel<TDTO>> First(ActionFilterModel args);
    }
}