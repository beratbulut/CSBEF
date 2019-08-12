using System;

namespace CSBEF.Core.Interfaces
{
    public interface IHubConnectedIdModel : ICloneable
    {
        int UserId { get; set; }
        string ConnectionId { get; set; }
    }
}