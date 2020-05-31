using CSBEF.Core.Concretes;
using System;

namespace CSBEF.Core.Interfaces
{
    public interface IDynamicServiceAction
    {
        IReturnModel<TResult> RunAction<TResult, TServiceParamsWithIdentifier>(
            ServiceParamsWithIdentifier<TServiceParamsWithIdentifier> args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>, IReturnModel<TResult>, IReturnModel<TResult>> invoker)
            where TServiceParamsWithIdentifier : class;
    }
}
