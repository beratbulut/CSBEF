using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models.HubModels
{
    public class HubConnectedIdModel : IHubConnectedIdModel
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}