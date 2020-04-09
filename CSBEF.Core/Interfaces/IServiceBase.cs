using System;
using System.Collections.Generic;
using CSBEF.Core.Models;

namespace CSBEF.Core.Interfaces {
    public interface IServiceBase<TPoco, TDTO> : IModuleService, IDisposable
    where TPoco : class, IEntityModelBase, new ()
    where TDTO : class, IDTOModelBase, new () {
        IRepositoryBase<TPoco> Repository { get; set; }
        IReturnModel<TDTO> First (GenericFilterModel<TDTO> filter);
        IReturnModel<TDTO> First (ActionFilterModel filter);
        IReturnModel<TDTO> FirstOrDefault (GenericFilterModel<TDTO> filter);
        IReturnModel<TDTO> FirstOrDefault (ActionFilterModel filter);
        IReturnModel<bool> Any (GenericFilterModel<TDTO> filter);
        IReturnModel<bool> Any (ActionFilterModel filter);
        IReturnModel<IList<TDTO>> List (GenericFilterModel<TDTO> filter);
        IReturnModel<IList<TDTO>> List (ActionFilterModel filter);
        IReturnModel<int> Count (ActionFilterModel filter);
    }
}