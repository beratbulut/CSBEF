using CSBEF.enums;

namespace CSBEF.Models.Interfaces
{
    public interface IEventInfo
    {
        string EventName { get; set; }

        string ModuleName { get; set; }

        string ServiceName { get; set; }

        string ActionName { get; set; }

        EventTypeEnum EventType { get; set; }
    }
}