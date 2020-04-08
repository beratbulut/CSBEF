using CSBEF.Core.Interfaces;

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

        public ReturnModel<TResult> EventHandler<TResult, TParam>(TParam data)
        {
            if (Event != null)
            {
                return (ReturnModel<TResult>)Event.Invoke(data, EventInfo);
            }
            else
            {
                return null;
            }
        }
    }
}