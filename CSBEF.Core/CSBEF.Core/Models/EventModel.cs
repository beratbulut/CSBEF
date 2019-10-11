using CSBEF.Core.Interfaces;
using System.Threading.Tasks;

namespace CSBEF.Core.Models
{
    public class EventModel : IEventModel
    {
        public IEventInfo EventInfo { get; set; }

        public event EventDelegate Event;

        public EventModel()
        {
            EventInfo = new EventInfo();
        }

        public async Task<ReturnModel<TResult>> EventHandler<TResult, TParam>(TParam data)
        {
            if (Event != null)
            {
                return (ReturnModel<TResult>)await Event.Invoke(data, EventInfo).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }
    }
}