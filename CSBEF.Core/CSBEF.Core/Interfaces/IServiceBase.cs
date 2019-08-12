using CSBEF.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public interface IServiceBase
    {
    }

    public interface IServiceBase<TPoco, TDTO> : IModuleService, IDisposable
        where TPoco : class, IEntityModelBase, new()
        where TDTO : class, IDTOModelBase, new()
    {
        IRepositoryBase<TPoco> Repository { get; set; }

        Task<IReturnModel<TDTO>> FirstAsync(GenericFilterModel<TDTO> filter);

        Task<IReturnModel<TDTO>> FirstAsync(ActionFilterModel filter);

        Task<IReturnModel<TDTO>> FirstOrDefaultAsync(GenericFilterModel<TDTO> filter);

        Task<IReturnModel<TDTO>> FirstOrDefaultAsync(ActionFilterModel filter);

        Task<IReturnModel<bool>> AnyAsync(GenericFilterModel<TDTO> filter);

        Task<IReturnModel<bool>> AnyAsync(ActionFilterModel filter);

        Task<IReturnModel<IList<TDTO>>> ListAsync(GenericFilterModel<TDTO> filter);

        Task<IReturnModel<IList<TDTO>>> ListAsync(ActionFilterModel filter);

        Task<IReturnModel<int>> CountAsync(ActionFilterModel filter);
    }
}