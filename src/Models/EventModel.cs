using CSBEF.Models.Interfaces;

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

        public IReturnModel<TResult> EventHandler<TResult, TParam>(TParam data)
        {
            if (TheEvent != null)
            {
                return (IReturnModel<TResult>)TheEvent.Invoke(data, EventInfo);
            }
            else
            {
                return null;
            }
        }
    }
}