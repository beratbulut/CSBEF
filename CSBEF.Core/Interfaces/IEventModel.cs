using CSBEF.Core.Models;

namespace CSBEF.Core.Interfaces {
    public delegate dynamic EventDelegate (dynamic data, IEventInfo eventInfo);

    public interface IEventModel {
        IEventInfo EventInfo { get; set; }

        event EventDelegate EventEvent;

        ReturnModel<TResult> EventHandler<TResult, TParam> (TParam data);
    }
}