using CSBEF.Core.Models.HubModels;
using System.Threading.Tasks;

namespace CSBEF.Core.Interfaces
{
    public interface IHubSyncDataService
    {
        Task<IReturnModel<bool>> OnSync<Tdata>(HubSyncDataModel<Tdata> data, string group = "");
    }
}