using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using System;

namespace CSBEF.Core.Concretes
{
    public class MainEventsAddInitializer : IModuleEventsAddInitializer
    {
        public void Start(IEventService eventService)
        {
            if (eventService == null)
                throw new ArgumentNullException(nameof(eventService));

            eventService.AddEvent("InComingToken", "Main", "Main", "InComingToken", EventTypeEnum.before);
            eventService.AddEvent("InComingHubClientData", "Main", "Main", "InComingHubClientData", EventTypeEnum.before);
        }
    }
}