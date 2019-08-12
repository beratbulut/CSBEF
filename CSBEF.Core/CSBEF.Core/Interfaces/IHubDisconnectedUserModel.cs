using System.Collections.Generic;

namespace CSBEF.Core.Interfaces
{
    public interface IHubDisconnectedUserModel
    {
        IHubUserModel DisconnectedUser { get; set; }
        IList<IHubUserModel> ConnectedUsers { get; set; }
    }
}