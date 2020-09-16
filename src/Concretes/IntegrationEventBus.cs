using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSBEF.Models;
using CSBEF.Interfaces;

namespace CSBEF.Concretes
{
    public static class IntegrationEventBus
    {
        private static readonly List<IIntegrationEventInfo> events = new List<IIntegrationEventInfo>();

        public static void AddEvent(string eventName)
        {
            events.Add(new IntegrationEventInfo(eventName));
        }

        public static void AddEvents(List<string> eventNames)
        {
            events.AddRange(eventNames.Select(s => new IntegrationEventInfo(s)).ToList());
        }

        public static IIntegrationEventInfo FindEvent(string eventName)
        {
            return events.Where(w => w.EventName == eventName).FirstOrDefault();
        }

        public static List<string> GetEventNames()
        {
            return events.Select(s => s.EventName).ToList();
        }

        public static async Task<IReturnModel<bool>> TriggerEvent(string eventName, IntegrationEventArgs args)
        {
            var getEvent = IntegrationEventBus.FindEvent(eventName);
            if (getEvent != null)
            {
                return await getEvent.EventHandler(args).ConfigureAwait(false);
            }

            return new ReturnModel<bool>().SendResult(true);
        }
    }
}