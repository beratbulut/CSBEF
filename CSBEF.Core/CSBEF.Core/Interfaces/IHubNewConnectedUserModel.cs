using System.Collections.Generic;

namespace CSBEF.Core.Interfaces
{
    public interface IHubNewConnectedUserModel
    {
        IHubUserModel ConnectedUser { get; set; }
        IList<IHubUserModel> ConnectedUsers { get; set; }
    }
}