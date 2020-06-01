using CSBEF.Core.Concretes;
using CSBEF.Core.Models.HubModels;
using System;

namespace CSBEF.Core.Interfaces
{
    public interface IDynamicServiceAction
    {
        IReturnModel<TResult> RunAction<TResult, TServiceParamsWithIdentifier, TSocketSyncDataType>(
            ServiceParamsWithIdentifier<TServiceParamsWithIdentifier> args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>, IReturnModel<TResult>, IReturnModel<TResult>> invoker, HubSyncDataModel<TSocketSyncDataType> hubSyncDataModel = null)
            where TServiceParamsWithIdentifier : class;

        IReturnModel<TResult> RunAction<TResult, TServiceParamsWithIdentifier>(
            ServiceParamsWithIdentifier<TServiceParamsWithIdentifier> args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>, IReturnModel<TResult>, IReturnModel<TResult>> invoker)
            where TServiceParamsWithIdentifier : class;
    }
}
