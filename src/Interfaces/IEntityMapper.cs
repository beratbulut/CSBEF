using Microsoft.EntityFrameworkCore;

namespace CSBEF.Interfaces
{
    public interface IEntityMapper
    {
        void Mapper(ModelBuilder modelBuilder);
    }
}