using System;
using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using CSBEF.Core.Models.HubModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSBEF.Core.Concretes {
    public class HubNotificationService : IHubNotificationService {
        #region Dependencies

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IReturnModel<bool>> _logger;

        #endregion Dependencies

        #region ctor

        public HubNotificationService (IServiceProvider serviceProvider, ILogger<IReturnModel<bool>> logger) {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        #endregion ctor

        #region Actions

        public IReturnModel<bool> OnNotify (NotificationModel data, string group = "") {
            IReturnModel<bool> rtn = new ReturnModel<bool> (_logger);

            try {
                var globalHub = _serviceProvider.GetService<IHubContext<GlobalHub>> ();

                if (!string.IsNullOrWhiteSpace (group)) {
                    globalHub.Clients.Group (group).SendAsync ("HubNotifiy", data).ConfigureAwait (false);
                } else {
                    globalHub.Clients.All.SendAsync ("HubNotifiy", data).ConfigureAwait (false);
                }
            } catch (CustomException ex) {
                rtn = rtn.SendError (GlobalError.TechnicalError, ex);
            }

            return rtn;
        }

        #endregion Actions
    }
}