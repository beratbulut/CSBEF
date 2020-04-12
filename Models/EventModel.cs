using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models {
    public class EventModel : IEventModel {
        public IEventInfo EventInfo { get; set; }

        public event EventDelegate EventEvent;

        public EventModel () {
            EventInfo = new EventInfo ();
        }

        public ReturnModel<TResult> EventHandler<TResult, TParam> (TParam data) {
            if (EventEvent != null) {
                return (ReturnModel<TResult>) EventEvent.Invoke (data, EventInfo);
            } else {
                return null;
            }
        }
    }
}