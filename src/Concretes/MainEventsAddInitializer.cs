using System.Threading.Tasks;
using CSBEF.enums;
using CSBEF.Interfaces;
using CSBEF.Helpers;

namespace CSBEF.Concretes
{
    public class MainEventsAddInitializer : IModuleEventsAddInitializer
    {
        public Task Start(IEventService eventService)
        {
            eventService.ThrowIfNull();

            eventService.AddEvent("InComingToken", "Main", "Main", "InComingToken", EventTypes.Before);
            eventService.AddEvent("InComingHubClientData", "Main", "Main", "InComingHubClientData", EventTypes.Before);

            return Task.CompletedTask;
        }
    }
}