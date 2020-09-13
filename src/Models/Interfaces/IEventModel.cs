namespace CSBEF.Models.Interfaces
{
    public delegate dynamic EventDelegate(dynamic data, IEventInfo eventInfo);

    public interface IEventModel
    {
        IEventInfo EventInfo { get; set; }

        event EventDelegate TheEvent;

        IReturnModel<TResult> EventHandler<TResult, TParam>(TParam data);
    }
}