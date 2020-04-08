using Microsoft.EntityFrameworkCore;

namespace CSBEF.Core.Interfaces
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}