using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    public interface IModuleEventsAddInitializer
    {
        Task Start(IEventService eventService);
    }
}