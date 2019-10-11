using CSBEF.Core.Interfaces;
using System.Collections.Generic;

namespace CSBEF.Core.Models.HubModels
{
    public class HubNewConnectedUserModel : IHubNewConnectedUserModel
    {
        public IHubUserModel ConnectedUser { get; set; }
        public IList<IHubUserModel> ConnectedUsers { get; } = new List<IHubUserModel>();
    }
}