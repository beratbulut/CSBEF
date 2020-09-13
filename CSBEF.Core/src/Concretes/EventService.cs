using System;
using System.Collections.Generic;
using System.Linq;
using CSBEF.Core.enums;
using CSBEF.Core.Models;
using CSBEF.Core.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Core.Concretes
{
    public class EventService : IEventService
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Tüm modüllerden elde edilen event tanımlarından oluşan listedir.
        /// </summary>
        /// <typeparam name="IEventModel">İçerisinde yer alan event tanımlarının kalıtım alacakları interface'dir.</typeparam>
        private readonly List<IEventModel> events = new List<IEventModel>();

        public EventService(IServiceProvider provider)
        {
            // TODO: To be translated into English
            // Tüm modüllerden elde edilmiş IModuleEventsAddInitializer sınıflarının instance'ları ServiceProvider içerisinden elde edilir.
            var eventAddInitializers = provider.GetServices<IModuleEventsAddInitializer>();

            // TODO: To be translated into English
            // Tüm modüllerden elde edilmiş IModuleEventsJoinInitializer sınıflarının instance'ları ServiceProvider içerisinden elde edilir.
            var eventJoinInitializers = provider.GetServices<IModuleEventsJoinInitializer>();

            // TODO: To be translated into English
            // Eğer elde edilmiş IModuleEventsAddInitializer instance'ları varsa, bunlarda dönülerek her birinin Start metodu tetiklenir.
            // Böylece her modülün kendi event tanımlarını listeye eklenmesi beklenir.
            // Eğer "eventAddInitializers" listesi boşsa, API uygulamasında hiç modül yok demektir.
            if (eventAddInitializers.Any())
            {
                foreach (var initializer in eventAddInitializers)
                {
                    initializer.Start(this);
                }
            }

            // TODO: To be translated into English
            // Eğer elde edilmiş IModuleEventsJoinInitializer instance'ları varsa, bunlarda dönülerek her birinin Start metodu tetiklenir.
            // Böylece modüllerin diğer modül event'larına kendilerini subscribe etmeleri beklenir.
            // Eğer "eventJoinInitializers" listesi boşsa, API uygulamasında hiç modül yok demektir.
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
            // TODO: To be translated into English
            // Gelen bilgilerle event tanımının daha önce eklenme durumu kontrol edilir.
            // Eğer daha önce eklenmişse herhangi bir işlem yapılmaz.
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