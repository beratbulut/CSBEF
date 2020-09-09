using System.Threading.Tasks;
using CSBEF.Models.Interfaces;

namespace CSBEF.Models
{
    public class IntegrationEventInfo : IIntegrationEventInfo
    {
        public event IntegrationEventDelegate EventObject;
        public string EventName { get; set; }

        public IntegrationEventInfo(string eventName)
        {
            this.EventName = eventName;
        }

        public async Task<IReturnModel<bool>> EventHandler(IntegrationEventArgs args)
        {
            IReturnModel<bool> rtn = new ReturnModel<bool>();

            if (EventObject == null)
            {
                return rtn.SendResult(true);
            }

            return await EventObject.Invoke(args).ConfigureAwait(false);
        }
    }
}