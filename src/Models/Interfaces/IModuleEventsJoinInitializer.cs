using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    public interface IModuleEventsJoinInitializer
    {
        Task Start(IEventService eventService);
    }
}