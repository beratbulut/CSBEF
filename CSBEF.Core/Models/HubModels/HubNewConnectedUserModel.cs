using System.Collections.Generic;
using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models.HubModels {
    public class HubNewConnectedUserModel : IHubNewConnectedUserModel {
        public IHubUserModel ConnectedUser { get; set; }
        public IList<IHubUserModel> ConnectedUsers { get; } = new List<IHubUserModel> ();
    }
}