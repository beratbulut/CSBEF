using CSBEF.Core.Interfaces;
using System.Collections.Generic;

namespace CSBEF.Core.Models.HubModels
{
    public class HubUserModel : IHubUserModel
    {
        public int Id { get; set; }
        public List<string> ConnectionId { get; set; }

        public HubUserModel()
        {
            ConnectionId = new List<string>();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}