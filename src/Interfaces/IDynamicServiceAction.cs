using System;
using System.Threading.Tasks;

namespace CSBEF.Interfaces
{
    public interface IDynamicServiceAction
    {
        Task<IReturnModel<TResult>> RunActionWithValidation<TResult, TParam>(
            TParam args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<TParam, IReturnModel<TResult>, Task<IReturnModel<TResult>>> invoker
        )
        where TParam : class;

        Task<IReturnModel<TResult>> RunAction<TResult, TParam>(
            TParam args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<TParam, IReturnModel<TResult>, Task<IReturnModel<TResult>>> invoker
        );

        Task<IReturnModel<TResult>> RunActionWithServiceParamsWithIdentifier<TResult, TParam, TParamChild>(
            TParam args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<TParam, IReturnModel<TResult>, Task<IReturnModel<TResult>>> invoker
        )
            where TParam : IServiceParamsWithIdentifier<TParamChild>
            where TParamChild : class;
    }
}