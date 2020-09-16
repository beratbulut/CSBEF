using System;
using System.Linq;
using System.Threading.Tasks;
using CSBEF.enums;
using CSBEF.Interfaces;
using CSBEF.Models;
using CSBEF.src.Helpers.Exceptions;
using Microsoft.Extensions.Logging;

namespace CSBEF.Helpers
{
    public class DynamicServiceAction : IDynamicServiceAction
    {
        private readonly ILogger<IDynamicServiceAction> logger;
        private readonly IEventService eventService;

        public DynamicServiceAction(ILogger<IDynamicServiceAction> logger, IEventService eventService)
        {
            this.logger = logger;
            this.eventService = eventService;
        }

        public async Task<IReturnModel<TResult>> RunActionWithValidation<TResult, TParam>(
            TParam args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<TParam, IReturnModel<TResult>, Task<IReturnModel<TResult>>> invoker
        )
            where TParam : class
        {
            args.ThrowIfNull();

            IReturnModel<TResult> rtn = new ReturnModel<TResult>();

            try
            {
                var modelValidation = args.ModelValidation();

                if (modelValidation.Any())
                {
                    rtn = rtn.SendError(GlobalErrors.ModelValidationFail);
                    Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.ModelValidationFail, "", logger, LogLevel.Error);
                    return rtn;
                }

                #region Before Event Handler

                var beforeEventHandler = await (await eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.Before").ConfigureAwait(false))
                    .EventHandler<bool, TParam>(args).ConfigureAwait(false);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.ErrorInfo.Status)
                    {
                        rtn.ErrorInfo = beforeEventHandler.ErrorInfo;
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.BeforeReturnedError, rtn.ErrorInfo.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (invoker != null)
                {
                    try
                    {
                        rtn = await invoker(args, rtn).ConfigureAwait(false);
                    }
                    catch (DynamicServiceActionException ex)
                    {
                        rtn = rtn.SendError(GlobalErrors.InvokerReturnedError);
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.InvokerReturnedError, ex.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                }

                #endregion

                #region After Event Handler

                var afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TResult>, TParam>
                {
                    DataToBeSent = rtn,
                    ActionParameter = args,
                    ModuleName = moduleName,
                    ServiceName = serviceName,
                    ActionName = actionName
                };
                var afterEventHandler = await (await eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.After").ConfigureAwait(false))
                    .EventHandler<TResult, IAfterEventParameterModel<IReturnModel<TResult>, TParam>>(afterEventParameterModel).ConfigureAwait(false);
                if (afterEventHandler != null)
                {
                    if (afterEventHandler.ErrorInfo.Status)
                    {
                        rtn.ErrorInfo = afterEventHandler.ErrorInfo;
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.AfterReturnedError, rtn.ErrorInfo.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                    else
                    {
                        rtn.Result = afterEventHandler.Result;
                    }
                }

                #endregion After Event Handler

                return rtn;
            }
            catch (DynamicServiceActionException ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError);
                Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.TechnicalError, ex.Message, logger, LogLevel.Error);
                return rtn;
            }
        }

        public async Task<IReturnModel<TResult>> RunAction<TResult, TParam>(
            TParam args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<TParam, IReturnModel<TResult>, Task<IReturnModel<TResult>>> invoker
        )
        {
            IReturnModel<TResult> rtn = new ReturnModel<TResult>();

            try
            {
                #region Before Event Handler

                var beforeEventHandler = await (await eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.Before").ConfigureAwait(false))
                    .EventHandler<bool, TParam>(args).ConfigureAwait(false);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.ErrorInfo.Status)
                    {
                        rtn.ErrorInfo = beforeEventHandler.ErrorInfo;
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.BeforeReturnedError, rtn.ErrorInfo.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (invoker != null)
                {
                    try
                    {
                        rtn = await invoker(args, rtn).ConfigureAwait(false);
                    }
                    catch (DynamicServiceActionException ex)
                    {
                        rtn = rtn.SendError(GlobalErrors.InvokerReturnedError);
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.InvokerReturnedError, ex.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                }

                #endregion

                #region After Event Handler

                var afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TResult>, TParam>
                {
                    DataToBeSent = rtn,
                    ActionParameter = args,
                    ModuleName = moduleName,
                    ServiceName = serviceName,
                    ActionName = actionName
                };
                var afterEventHandler = await (await eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.After").ConfigureAwait(false))
                    .EventHandler<TResult, IAfterEventParameterModel<IReturnModel<TResult>, TParam>>(afterEventParameterModel).ConfigureAwait(false);
                if (afterEventHandler != null)
                {
                    if (afterEventHandler.ErrorInfo.Status)
                    {
                        rtn.ErrorInfo = afterEventHandler.ErrorInfo;
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.AfterReturnedError, rtn.ErrorInfo.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                    else
                    {
                        rtn.Result = afterEventHandler.Result;
                    }
                }

                #endregion After Event Handler

                return rtn;
            }
            catch (DynamicServiceActionException ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError);
                Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.TechnicalError, ex.Message, logger, LogLevel.Error);
                return rtn;
            }
        }

        public async Task<IReturnModel<TResult>> RunActionWithServiceParamsWithIdentifier<TResult, TParam, TParamChild>(
            TParam args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<TParam, IReturnModel<TResult>, Task<IReturnModel<TResult>>> invoker
        )
            where TParam : IServiceParamsWithIdentifier<TParamChild>
            where TParamChild : class
        {
            args.ThrowIfNull();

            IReturnModel<TResult> rtn = new ReturnModel<TResult>();

            try
            {
                var modelValidation = args.Param.ModelValidation();

                if (modelValidation.Any())
                {
                    rtn = rtn.SendError(GlobalErrors.ModelValidationFail);
                    Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.ModelValidationFail, "", logger, LogLevel.Error);
                    return rtn;
                }

                #region Before Event Handler

                var beforeEventHandler = await (await eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.Before").ConfigureAwait(false))
                    .EventHandler<bool, TParam>(args).ConfigureAwait(false);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.ErrorInfo.Status)
                    {
                        rtn.ErrorInfo = beforeEventHandler.ErrorInfo;
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.BeforeReturnedError, rtn.ErrorInfo.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (invoker != null)
                {
                    try
                    {
                        rtn = await invoker(args, rtn).ConfigureAwait(false);
                    }
                    catch (DynamicServiceActionException ex)
                    {
                        rtn = rtn.SendError(GlobalErrors.InvokerReturnedError);
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.InvokerReturnedError, ex.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                }

                #endregion

                #region After Event Handler

                var afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TResult>, TParam>
                {
                    DataToBeSent = rtn,
                    ActionParameter = args,
                    ModuleName = moduleName,
                    ServiceName = serviceName,
                    ActionName = actionName
                };
                var afterEventHandler = await (await eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.After").ConfigureAwait(false))
                    .EventHandler<TResult, IAfterEventParameterModel<IReturnModel<TResult>, TParam>>(afterEventParameterModel).ConfigureAwait(false);
                if (afterEventHandler != null)
                {
                    if (afterEventHandler.ErrorInfo.Status)
                    {
                        rtn.ErrorInfo = afterEventHandler.ErrorInfo;
                        Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.AfterReturnedError, rtn.ErrorInfo.Message, logger, LogLevel.Error);
                        return rtn;
                    }
                    else
                    {
                        rtn.Result = afterEventHandler.Result;
                    }
                }

                #endregion After Event Handler

                return rtn;
            }
            catch (DynamicServiceActionException ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError);
                Tools.WriteLoggerForService(moduleName, serviceName + "(DynamicServiceAction)", actionName, GlobalErrors.TechnicalError, ex.Message, logger, LogLevel.Error);
                return rtn;
            }
        }

    }
}