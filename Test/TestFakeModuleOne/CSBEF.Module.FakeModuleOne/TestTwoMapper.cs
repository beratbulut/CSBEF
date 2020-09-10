using CSBEF.Core.Helpers;
using CSBEF.Helpers;
using CSBEF.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Module.FakeModuleOne
{
    public class TestTwoMapper : IEntityMapper
    {
        public void Mapper(ModelBuilder modelBuilder)
        {
            modelBuilder.ThrowIfNull();

            modelBuilder.Entity<TestTwo>(entity =>
            {
                entity.ToTable("FakeModuleOne_TestTwo");

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