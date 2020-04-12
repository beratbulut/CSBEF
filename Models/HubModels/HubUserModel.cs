using System.Collections.Generic;
using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models.HubModels {
    public class HubUserModel : IHubUserModel {
        public int Id { get; set; }
        public List<string> ConnectionId { get; } = new List<string> ();

        public HubUserModel () {
            ConnectionId = new List<string> ();
        }

        public object Clone () {
            return MemberwiseClone ();
        }
    }
}