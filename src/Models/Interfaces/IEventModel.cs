using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    public delegate Task<dynamic> EventDelegate(dynamic data, IEventInfo eventInfo);

    public interface IEventModel
    {
        IEventInfo EventInfo { get; set; }

        event EventDelegate TheEvent;

        Task<IReturnModel<TResult>> EventHandler<TResult, TParam>(TParam data);
    }
}