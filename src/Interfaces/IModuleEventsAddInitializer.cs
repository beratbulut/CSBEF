using System.Threading.Tasks;

namespace CSBEF.Interfaces
{
    public interface IModuleEventsAddInitializer
    {
        Task Start(IEventService eventService);
    }
}