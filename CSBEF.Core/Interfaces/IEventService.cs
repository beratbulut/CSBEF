using System.Collections.Generic;
using CSBEF.Core.Enums;

namespace CSBEF.Core.Interfaces {
    public interface IEventService {
        IEventModel GetEvent (string moduleName, string eventName);

        void AddEvent (string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType, bool denyHubUse = false, List<string> accessHubs = null);

        List<IEventModel> GetAllEvents ();
    }
}