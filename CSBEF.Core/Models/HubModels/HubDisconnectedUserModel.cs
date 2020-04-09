using System.Collections.Generic;
using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models.HubModels {
    public class HubDisconnectedUserModel : IHubDisconnectedUserModel {
        public IHubUserModel DisconnectedUser { get; set; }
        public IList<IHubUserModel> ConnectedUsers { get; } = new List<IHubUserModel> ();
    }
}