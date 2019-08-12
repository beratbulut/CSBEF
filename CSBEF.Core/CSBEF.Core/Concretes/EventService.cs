using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSBEF.Core.Concretes
{
    public class EventService : IEventService
    {
        private List<IEventModel> _events = new List<IEventModel>();

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
            #region Variables

            IEventModel findEvent = null;

            #endregion Variables

            #region Action Body

            findEvent = _events.FirstOrDefault(i => i.EventInfo.ModuleName == moduleName && i.EventInfo.EventName == eventName);

            #endregion Action Body

            #region Clear Memory

            moduleName = null;
            eventName = null;

            #endregion Clear Memory

            return findEvent ?? new EventModel();
        }

        public void AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType, bool denyHubUse = false, List<string> accessHubs = null)
        {
            #region Action Body

            if (_events.Any(i => i.EventInfo.ModuleName == moduleName && i.EventInfo.EventName == eventName))
                return;

            _events.Add(new EventModel()
            {
                EventInfo = new EventInfo
                {
                    EventName = eventName,
                    ModuleName = moduleName,
                    ServiceName = serviceName,
                    ActionName = actionName,
                    EventType = eventType,
                    DenyHubUse = denyHubUse,
                    AccessHubs = accessHubs ?? new List<string>()
                }
            });

            #endregion Action Body
        }

        public List<IEventModel> GetAllEvents()
        {
            return _events;
        }
    }
}