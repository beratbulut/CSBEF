using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using System.Collections.Generic;

namespace CSBEF.Core.Models
{
    public class EventInfo : IEventInfo
    {
        public string EventName { get; set; }
        public string ModuleName { get; set; }
        public string ServiceName { get; set; }
        public string ActionName { get; set; }
        public bool DenyHubUse { get; set; } = false;
        public List<string> AccessHubs { get; } = new List<string>();
        public EventTypeEnum EventType { get; set; }
    }
}