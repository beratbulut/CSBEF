using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using CSBEF.Core.Models.HubModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CSBEF.Core.Concretes
{
    [Authorize]
    public class GlobalHub : Hub
    {
        private readonly ILogger<ILog> _logger;
        private readonly IEventService _eventService;

        public GlobalHub(ILogger<ILog> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        public override async Task OnConnectedAsync()
        {
            IHubUserModel user = new HubUserModel
            {
                Id = Context.User.Claims.First(i => i.Type == ClaimTypes.Name).Value.ToInt(0)
            };

            user.ConnectionId.Add(Context.ConnectionId);

            await base.OnConnectedAsync().ConfigureAwait(false);
            var addNewUser = await HubConnectedUserStore.Add(user).ConfigureAwait(false);

            await Groups.AddToGroupAsync(Context.ConnectionId, "connection_" + Context.ConnectionId).ConfigureAwait(false);
            await Groups.AddToGroupAsync(Context.ConnectionId, "user_" + user.Id).ConfigureAwait(false);

            var connectedUsersList = await HubConnectedUserStore.ConnectedUserList().ConfigureAwait(false);
            await Clients.Group("connection_" + Context.ConnectionId).SendAsync("GiveConnectedUsersList", connectedUsersList).ConfigureAwait(false);

            if (addNewUser)
            {
                await Clients.All.SendAsync("ConnectedNewClient", user).ConfigureAwait(false);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await HubConnectedUserStore.FindUser(Context.ConnectionId).ConfigureAwait(false);

            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
            var removeUser = await HubConnectedUserStore.Remove(Context.ConnectionId).ConfigureAwait(false);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "connection_" + Context.ConnectionId).ConfigureAwait(false);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "user_" + user.Id).ConfigureAwait(false);

            if (removeUser)
            {
                user.ConnectionId.Clear();
                await Clients.All.SendAsync("DisconnectedClient", user).ConfigureAwait(false);
            }
        }

        public async Task<IList<IHubUserModel>> GetConnectedUsersList()
        {
            return await HubConnectedUserStore.ConnectedUserList().ConfigureAwait(false);
        }

        public async Task<IReturnModel<SendModuleDataModel>> InComingClientData(InComingClientDataModel data)
        {
            IReturnModel<SendModuleDataModel> rtn = new ReturnModel<SendModuleDataModel>(_logger);

            try
            {
                var userId = Tools.GetTokenNameClaim(Context);
                var tokenId = Tools.GetTokenIdClaim(Context);
                var serviceParam = new ServiceParamsWithIdentifier<InComingClientDataModel>(data, userId, tokenId);
                var exec = await _eventService.GetEvent("Main", "InComingHubClientData").EventHandler<SendModuleDataModel, ServiceParamsWithIdentifier<InComingClientDataModel>>(serviceParam).ConfigureAwait(false);
                if (exec.Error.Status)
                {
                    rtn.Error = exec.Error;
                }
                else
                {
                    rtn.Result = exec.Result;
                }
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }
    }
}