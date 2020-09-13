using System.Collections.Generic;
using CSBEF.enums;

namespace CSBEF.Models.Interfaces
{
    public interface IEventService
    {
        IEventModel GetEvent(string moduleName, string eventName);

        void AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType);

        List<IEventModel> GetAllEvents();
    }
}