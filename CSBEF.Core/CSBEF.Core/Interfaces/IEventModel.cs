using CSBEF.Core.Models;
using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public delegate Task<dynamic> EventDelegate(dynamic data, IEventInfo eventInfo);

    public interface IEventModel
    {
        IEventInfo EventInfo { get; set; }

        event EventDelegate Event;

        Task<ReturnModel<TResult>> EventHandler<TResult, TParam>(TParam data);
    }
}