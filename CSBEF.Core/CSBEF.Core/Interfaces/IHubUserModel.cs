using System;
using System.Collections.Generic;

namespace CSBEF.Core.Interfaces
{
    public interface IHubUserModel : ICloneable
    {
        int Id { get; set; }
        List<string> ConnectionId { get; set; }
    }
}