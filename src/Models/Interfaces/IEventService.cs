using System.Collections.Generic;
using System.Threading.Tasks;
using CSBEF.enums;

namespace CSBEF.Models.Interfaces
{
    public interface IEventService
    {
        Task<IEventModel> GetEvent(string moduleName, string eventName);

        Task AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypes eventType);

        Task<List<IEventModel>> GetAllEvents();
    }
}