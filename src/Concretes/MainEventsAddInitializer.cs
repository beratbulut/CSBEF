using System.Threading.Tasks;
using CSBEF.enums;
using CSBEF.Models.Interfaces;
using CSBEF.Helpers;

namespace CSBEF.Concretes
{
    public class MainEventsAddInitializer : IModuleEventsAddInitializer
    {
        public Task Start(IEventService eventService)
        {
            eventService.ThrowIfNull();

            eventService.AddEvent("InComingToken", "Main", "Main", "InComingToken", EventTypeEnum.Before);
            eventService.AddEvent("InComingHubClientData", "Main", "Main", "InComingHubClientData", EventTypeEnum.Before);

            return Task.CompletedTask;
        }
    }
}