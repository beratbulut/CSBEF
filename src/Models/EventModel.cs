using System.Threading.Tasks;
using CSBEF.Interfaces;

namespace CSBEF.Models
{
    public class EventModel : IEventModel
    {
        public IEventInfo EventInfo { get; set; }

        public event EventDelegate TheEvent;

        public EventModel()
        {
            EventInfo = new EventInfo();
        }

        public async Task<IReturnModel<TResult>> EventHandler<TResult, TParam>(TParam data)
        {
            if (TheEvent != null)
            {
                return await TheEvent.Invoke(data, EventInfo).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }
    }
}