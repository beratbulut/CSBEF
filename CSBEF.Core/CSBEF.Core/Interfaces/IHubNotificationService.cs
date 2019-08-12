using CSBEF.Core.Models.HubModels;
using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public interface IHubNotificationService
    {
        Task<IReturnModel<bool>> OnNotify(NotificationModel data, string group = "");
    }
}