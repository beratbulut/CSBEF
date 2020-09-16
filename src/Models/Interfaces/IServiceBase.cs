using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace CSBEF.Models.Interfaces
{
    public interface IServiceBase : IDisposable
    {

    }

    public interface IServiceBase<TPoco, TDTO> : IModuleService, IDisposable
        where TPoco : class, IEntityModelBase, new()
        where TDTO : class, IDtoModelBase, new()
    {
        string ModuleName { get; set; }
        string ServiceName { get; set; }
        IRepositoryBaseWithCud<TPoco> Repository { get; set; }
        Task<IReturnModel<TDTO>> FirstAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default);
        Task<IReturnModel<TDTO>> FirstAsync(ActionFilterModel args, CancellationToken cancellationToken = default);
        Task<IReturnModel<TDTO>> FirstOrDefaultAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default);
        Task<IReturnModel<TDTO>> FirstOrDefaultAsync(ActionFilterModel args, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> AnyAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> AnyAsync(ActionFilterModel args, CancellationToken cancellationToken = default);
        Task<IReturnModel<IList<TDTO>>> ListAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default);
        Task<IReturnModel<IList<TDTO>>> ListAsync(ActionFilterModel args, CancellationToken cancellationToken = default);
        Task<IReturnModel<int>> CountAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default);
        Task<IReturnModel<int>> CountAsync(ActionFilterModel args, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> AddAsync(ServiceParamsWithIdentifier<TDTO> args, bool useSave = true, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> UpdateAsync(ServiceParamsWithIdentifier<TDTO> args, bool useSave = true, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> ChangeStatusAsync(ServiceParamsWithIdentifier<ChangeStatusModel> args, bool useSave = true, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> AddRangeAsync(ServiceParamsWithIdentifier<List<TDTO>> args, bool withTransaction = true, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> UpdateRangeAsync(ServiceParamsWithIdentifier<List<TDTO>> args, bool withTransaction = true, CancellationToken cancellationToken = default);
        Task<IReturnModel<bool>> ChangeStatusRangeAsync(ServiceParamsWithIdentifier<List<ChangeStatusModel>> args, bool withTransaction = true, CancellationToken cancellationToken = default);
    }
}