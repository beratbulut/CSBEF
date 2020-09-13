using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Models.Interfaces
{
    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection);
    }
}