using CSBEF.Core.Helpers;
using CSBEF.Helpers;
using CSBEF.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Module.FakeModuleOne
{
    public class TestOneMapper : IEntityMapper
    {
        public void Mapper(ModelBuilder modelBuilder)
        {
            modelBuilder.ThrowIfNull();

            modelBuilder.Entity<TestOne>(entity =>
            {
                entity.ToTable("FakeModuleOne_TestOne");

                entity.InjectDefaultProperties();

                entity.Property(p => p.TestCol1)
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                entity.Property(p => p.TestCol2)
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);
            });
        }
    }
}