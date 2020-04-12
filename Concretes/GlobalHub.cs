using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using CSBEF.Core.Models.HubModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CSBEF.Core.Concretes {
    [Authorize]
    public class GlobalHub : Hub {
        private readonly ILogger<IReturnModel<bool>> _logger;
        private readonly IEventService _eventService;

        public GlobalHub (ILogger<IReturnModel<bool>> logger, IEventService eventService) {
            _logger = logger;
            _eventService = eventService;
        }

        public override async Task OnConnectedAsync () {
            IHubUserModel user = new HubUserModel {
                Id = Context.User.Claims.First (i => i.Type == ClaimTypes.Name).Value.ToInt (0)
            };

            user.ConnectionId.Add (Context.ConnectionId);

            await base.OnConnectedAsync ().ConfigureAwait (false);
            var addNewUser = await HubConnectedUserStore.Add (user).ConfigureAwait (false);

            await Groups.AddToGroupAsync (Context.ConnectionId, "connection_" + Context.ConnectionId).ConfigureAwait (false);
            await Groups.AddToGroupAsync (Context.ConnectionId, "user_" + user.Id).ConfigureAwait (false);

            var connectedUsersList = await HubConnectedUserStore.ConnectedUserList ().ConfigureAwait (false);
            await Clients.Group ("connection_" + Context.ConnectionId).SendAsync ("GiveConnectedUsersList", connectedUsersList).ConfigureAwait (false);

            if (addNewUser) {
                await Clients.All.SendAsync ("ConnectedNewClient", user).ConfigureAwait (false);
            }
        }

        public override async Task OnDisconnectedAsync (Exception exception) {
            var user = await HubConnectedUserStore.FindUser (Context.ConnectionId).ConfigureAwait (false);

            await base.OnDisconnectedAsync (exception).ConfigureAwait (false);
            var removeUser = await HubConnectedUserStore.Remove (Context.ConnectionId).ConfigureAwait (false);

            await Groups.RemoveFromGroupAsync (Context.ConnectionId, "connection_" + Context.ConnectionId).ConfigureAwait (false);
            await Groups.RemoveFromGroupAsync (Context.ConnectionId, "user_" + user.Id).ConfigureAwait (false);

            if (removeUser) {
                user.ConnectionId.Clear ();
                await Clients.All.SendAsync ("DisconnectedClient", user).ConfigureAwait (false);
            }
        }

        public static async Task<IList<IHubUserModel>> GetConnectedUsersList () {
            return await HubConnectedUserStore.ConnectedUserList ().ConfigureAwait (false);
        }

        public IReturnModel<SendModuleDataModel> InComingClientData (InComingClientDataModel data) {
            IReturnModel<SendModuleDataModel> rtn = new ReturnModel<SendModuleDataModel> (_logger);

            try {
                var userId = Tools.GetTokenNameClaim (Context);
                var tokenId = Tools.GetTokenIdClaim (Context);
                var serviceParam = new ServiceParamsWithIdentifier<InComingClientDataModel> (data, userId, tokenId);
                var exec = _eventService.GetEvent ("Main", "InComingHubClientData").EventHandler<SendModuleDataModel, ServiceParamsWithIdentifier<InComingClientDataModel>> (serviceParam);
                if (exec.ErrorInfo.Status) {
                    rtn.ErrorInfo = exec.ErrorInfo;
                } else {
                    rtn.Result = exec.Result;
                }
            } catch (CustomException ex) {
                rtn = rtn.SendError (GlobalError.TechnicalError, ex);
            }

            return rtn;
        }
    }
}