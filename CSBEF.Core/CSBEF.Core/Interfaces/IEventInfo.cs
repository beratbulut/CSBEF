using CSBEF.Core.Enums;
using System.Collections.Generic;

namespace CSBEF.Core.Interfaces
{
    public interface IEventInfo
    {
        string EventName { get; set; }
        string ModuleName { get; set; }
        string ServiceName { get; set; }
        string ActionName { get; set; }
        bool DenyHubUse { get; set; }
        List<string> AccessHubs { get; set; }
        EventTypeEnum EventType { get; set; }
    }
}