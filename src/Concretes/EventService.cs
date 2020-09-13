using System;
using System.Collections.Generic;
using System.Linq;
using CSBEF.enums;
using CSBEF.Models;
using CSBEF.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Concretes
{
    public class EventService : IEventService
    {
        private readonly List<IEventModel> events = new List<IEventModel>();

        public EventService(IServiceProvider provider)
        {
            var eventAddInitializers = provider.GetServices<IModuleEventsAddInitializer>();
            var eventJoinInitializers = provider.GetServices<IModuleEventsJoinInitializer>();

            if (eventAddInitializers.Any())
            {
                foreach (var initializer in eventAddInitializers)
                {
                    initializer.Start(this);
                }
            }

            if (eventJoinInitializers.Any())
            {
                foreach (var initializer in eventJoinInitializers)
                {
                    initializer.Start(this);
                }
            }
        }

        public IEventModel GetEvent(string moduleName, string eventName)
        {
            var findEvent = events.FirstOrDefault(i => i.EventInfo.ModuleName == moduleName && i.EventInfo.EventName == eventName); ;

            return findEvent ?? new EventModel();
        }

        public void AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType)
        {
            if (events.Any(i => i.EventInfo.ModuleName == moduleName && i.EventInfo.EventName == eventName))
            {
                return;
            }

            events.Add(new EventModel()
            {
                EventInfo = new EventInfo
                {
                    EventName = eventName,
                    ModuleName = moduleName,
                    ServiceName = serviceName,
                    ActionName = actionName,
                    EventType = eventType
                }
            });
        }

        public List<IEventModel> GetAllEvents()
        {
            return events;
        }
    }
}