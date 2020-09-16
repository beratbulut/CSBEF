using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Interfaces
{
    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection);
    }
}