using System.Threading.Tasks;

namespace CSBEF.Interfaces
{
    public interface IModuleEventsJoinInitializer
    {
        Task Start(IEventService eventService);
    }
}