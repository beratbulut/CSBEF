using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using CSBEF.Core.Models.HubModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CSBEF.Core.Concretes
{
    public class HubSyncDataService : IHubSyncDataService
    {
        #region Dependencies

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ILog> _logger;

        #endregion Dependencies

        #region ctor

        public HubSyncDataService(IServiceProvider serviceProvider, ILogger<ILog> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        #endregion ctor

        #region Actions

        public IReturnModel<bool> OnSync<T>(HubSyncDataModel<T> data, string group = "")
        {
            IReturnModel<bool> rtn = new ReturnModel<bool>(_logger);

            try
            {
                var globalHub = _serviceProvider.GetService<IHubContext<GlobalHub>>();

                if (!string.IsNullOrWhiteSpace(group))
                {
                    globalHub.Clients.Group(group).SendAsync("HubSyncData", data).ConfigureAwait(false);
                }
                else
                {
                    globalHub.Clients.All.SendAsync("HubSyncData", data).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        #endregion Actions
    }
}