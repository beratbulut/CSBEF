using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CSBEF.Core.Concretes
{
    public class DynamicServiceAction : IDynamicServiceAction
    {
        private ILogger<ILog> _logger;
        private IEventService _eventService;

        public DynamicServiceAction(ILogger<ILog> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        public IReturnModel<TResult> RunAction<TResult, TServiceParamsWithIdentifier>
            (
            ServiceParamsWithIdentifier<TServiceParamsWithIdentifier> args,
            string actionName,
            string serviceName,
            string moduleName,
            Func<ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>, IReturnModel<TResult>, IReturnModel<TResult>> invoker)
            where TServiceParamsWithIdentifier : class
        {
            IReturnModel<TResult> rtn = new ReturnModel<TResult>(_logger);

            try
            {
                var modelValidation = args.Param.ModelValidation();

                if (modelValidation.Any())
                {
                    rtn = rtn.SendError(GlobalErrors.ModelValidationFail);
                    return rtn;
                }

                #region Before Event Handler

                var beforeEventHandler = _eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.Before").EventHandler<bool, ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>>(args);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        return rtn;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if(invoker != null)
                    rtn = invoker(args, rtn);

                #endregion

                #region After Event Handler

                var afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TResult>, ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>>
                {
                    DataToBeSent = rtn,
                    ActionParameter = args,
                    ModuleName = moduleName,
                    ServiceName = serviceName,
                    ActionName = actionName
                };
                var afterEventHandler = _eventService.GetEvent(moduleName, $"{serviceName}.{actionName}.After")
                    .EventHandler<TResult, IAfterEventParameterModel<IReturnModel<TResult>, ServiceParamsWithIdentifier<TServiceParamsWithIdentifier>>>(afterEventParameterModel);
                if (afterEventHandler != null)
                {
                    if (afterEventHandler.Error.Status)
                    {
                        rtn.Error = afterEventHandler.Error;
                        return rtn;
                    }
                    else
                    {
                        rtn.Result = afterEventHandler.Result;
                    }
                }

                #endregion After Event Handler
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }
    }
}
