using System.Collections.Generic;
using CSBEF.Models.Interfaces;
using CSBEF.enums;

namespace CSBEF.Models
{
    public class EventInfo : IEventInfo
    {
        public string EventName { get; set; }

        public string ModuleName { get; set; }

        public string ServiceName { get; set; }

        public string ActionName { get; set; }

        public bool DenyHubUse { get; set; }

        public List<string> AccessHubs { get; } = new List<string>();

        public EventTypes EventType { get; set; }
    }
}