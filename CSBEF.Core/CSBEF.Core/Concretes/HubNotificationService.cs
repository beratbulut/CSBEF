using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using CSBEF.Core.Models.HubModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CSBEF.Core.Concretes
{
    public class HubNotificationService : IHubNotificationService
    {
        #region Dependencies

        private IServiceProvider _serviceProvider;
        private ILogger<ILog> _logger;

        #endregion Dependencies

        #region ctor

        public HubNotificationService(IServiceProvider serviceProvider, ILogger<ILog> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        #endregion ctor

        #region Actions

        public async Task<IReturnModel<bool>> OnNotify(NotificationModel data, string group = "")
        {
            IReturnModel<bool> rtn = new ReturnModel<bool>(_logger);

            try
            {
                var globalHub = _serviceProvider.GetService<IHubContext<GlobalHub>>();

                if (!string.IsNullOrWhiteSpace(group))
                {
                    await globalHub.Clients.Group(group).SendAsync("HubNotifiy", data);
                }
                else
                {
                    await globalHub.Clients.All.SendAsync("HubNotifiy", data);
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