using CSBEF.Core.Models.HubModels;

namespace CSBEF.Core.Interfaces
{
    public interface IHubSyncDataService
    {
        IReturnModel<bool> OnSync<T>(HubSyncDataModel<T> data, string group = "");
    }
}