using CSBEF.Core.Models.HubModels;
using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public interface IHubSyncDataService
    {
        Task<IReturnModel<bool>> OnSync<T>(HubSyncDataModel<T> data, string group = "");
    }
}