using Microsoft.EntityFrameworkCore;

namespace CSBEF.Models.Interfaces
{
    public interface IEntityMapper
    {
        void Mapper(ModelBuilder modelBuilder);
    }
}