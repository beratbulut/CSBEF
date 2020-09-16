using CSBEF.enums;

namespace CSBEF.Interfaces
{
    public interface IEventInfo
    {
        string EventName { get; set; }

        string ModuleName { get; set; }

        string ServiceName { get; set; }

        string ActionName { get; set; }

        EventTypes EventType { get; set; }
    }
}