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
        private ILogger<ILog> _logger;
        private IEventService _eventService;

        public GlobalHub(ILogger<ILog> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        public override async Task OnConnectedAsync()
        {
            IHubUserModel user = new HubUserModel
            {
                Id = Context.User.Claims.First(i => i.Type == ClaimTypes.Name).Value.ToInt(0),
                ConnectionId = new List<string>
                {
                    Context.ConnectionId
                }
            };

            await base.OnConnectedAsync();
            var addNewUser = await HubConnectedUserStore.Add(user);

            await Groups.AddToGroupAsync(Context.ConnectionId, "connection_" + Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "user_" + user.Id);

            var connectedUsersList = await HubConnectedUserStore.ConnectedUserList();
            await Clients.Group("connection_" + Context.ConnectionId).SendAsync("GiveConnectedUsersList", connectedUsersList);

            if (addNewUser)
            {
                await Clients.All.SendAsync("ConnectedNewClient", user);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await HubConnectedUserStore.FindUser(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
            var removeUser = await HubConnectedUserStore.Remove(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "connection_" + Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "user_" + user.Id);

            if (removeUser)
            {
                user.ConnectionId.Clear();
                await Clients.All.SendAsync("DisconnectedClient", user);
            }
        }

        public async Task<IList<IHubUserModel>> GetConnectedUsersList()
        {
            return await HubConnectedUserStore.ConnectedUserList();
        }

        public async Task<IReturnModel<SendModuleDataModel>> InComingClientData(InComingClientDataModel data)
        {
            IReturnModel<SendModuleDataModel> rtn = new ReturnModel<SendModuleDataModel>(_logger);

            try
            {
                var userId = Tools.GetTokenNameClaim(Context);
                var tokenId = Tools.GetTokenIdClaim(Context);
                var serviceParam = new ServiceParamsWithIdentifier<InComingClientDataModel>(data, userId, tokenId);
                var exec = await _eventService.GetEvent("Main", "InComingHubClientData").EventHandler<SendModuleDataModel, ServiceParamsWithIdentifier<InComingClientDataModel>>(serviceParam);
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