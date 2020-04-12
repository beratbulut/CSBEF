using CSBEF.Core.Models.HubModels;

namespace CSBEF.Core.Interfaces {
    public interface IHubNotificationService {
        IReturnModel<bool> OnNotify (NotificationModel data, string group = "");
    }
}