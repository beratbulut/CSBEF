using CSBEF.Core.Enums;
using System.Collections.Generic;

namespace CSBEF.Core.Interfaces
{
    public interface IEventService
    {
        IEventModel GetEvent(string moduleName, string eventName);

        void AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType, bool denyHubUse = false, List<string> accessHubs = null);

        List<IEventModel> GetAllEvents();
    }
}