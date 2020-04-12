using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Core.Interfaces {
    public interface IModuleInitializer {
        void Init (IServiceCollection serviceCollection);
    }
}