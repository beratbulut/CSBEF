using System.Collections.Generic;
using CSBEF.Core.Enums;

namespace CSBEF.Core.Interfaces {
    public interface IEventInfo {
        string EventName { get; set; }
        string ModuleName { get; set; }
        string ServiceName { get; set; }
        string ActionName { get; set; }
        bool DenyHubUse { get; set; }
        List<string> AccessHubs { get; }
        EventTypeEnum EventType { get; set; }
    }
}