using CSBEF.Core.Helpers;
using CSBEF.Helpers;
using CSBEF.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Module.FakeModuleOne
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF unit test için kullanılan test modülünün, 2 test tablosunun 2. tablosuna ait mapper nesnesidir.
    /// </summary>
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