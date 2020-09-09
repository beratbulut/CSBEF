using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSBEF.Models;
using CSBEF.Models.Interfaces;

namespace CSBEF.Concretes
{
    /// <summary>
    /// TODO: To be translated into English
    /// Entegrasyon işleminde her adım için bir öncesi birde sonrası şeklinde iki olay tetiklenmektedir.
    /// Bu olaylar sayesinde entegrasyon sürecine gerek modüllerden, gerekse de diğer namespace'ler üzerinden dahil olunabilir.
    /// Bu static sınıf, bu adımlarla ilgili tüm olayların havuzunu tutmakta ve yönetmektedir.
    /// İhtiyaç anında bu olaylara bu havuz üzerinden ulaşılmakta ve tetiklenmektedir.
    /// </summary>
    public static class IntegrationEventBus
    {
        /// <summary>
        /// TODO: To be translated into English
        /// İlgili tüm olayların bulunduğu liste.
        /// </summary>
        private static readonly List<IIntegrationEventInfo> events = new List<IIntegrationEventInfo>();

        /// <summary>
        /// TODO: To be translated into English
        /// Listeye olay eklemek için kullanılan metottur.
        /// </summary>
        /// <param name="eventName">Eklenecek yeni olayın adı.</param>
        public static void AddEvent(string eventName)
        {
            events.Add(new IntegrationEventInfo(eventName));
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Listeye bir kerede birden fazla olay eklemek için kullanılan metottur.
        /// </summary>
        /// <param name="eventNames">Eklenecek olayların isim listesi</param>
        public static void AddEvents(List<string> eventNames)
        {
            events.AddRange(eventNames.Select(s => new IntegrationEventInfo(s)).ToList());
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Olay adıyla listeden ilgili olayı bulmaya yarayan metottur.
        /// </summary>
        /// <param name="eventName">Aranacak olay adı</param>
        /// <returns>Bulunan olayın info modeli veya null</returns>
        public static IIntegrationEventInfo FindEvent(string eventName)
        {
            return events.Where(w => w.EventName == eventName).FirstOrDefault();
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Olay listesindeki tüm olayların isimlerinden oluşan listeyi elde etmek için kullanılan metottur.
        /// </summary>
        /// <returns>Listedeki tüm olayların isim listesi</returns>
        public static List<string> GetEventNames()
        {
            return events.Select(s => s.EventName).ToList();
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Normal şartlarda "FindEvent" ile olay bulunmalı ve bu olay üzerinden handle edilmelidir.
        /// Ancak bu metot sayesinde sadece event adı verilerek olayın bulunması ve handle edilmesi sağlanır.
        /// </summary>
        /// <param name="eventName">Tetiklenecek olayın adı</param>
        /// <param name="args">Tetiklenirken kullanılacak argumanlar</param>
        /// <returns>Olaya abone olanlardan dönen ResultModel</returns>
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