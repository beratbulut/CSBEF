using CSBEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSBEF.Helpers
{
    public static class ModelBuilderSetupDefaultProperties
    {
        public static void InjectDefaultProperties<T>(this EntityTypeBuilder<T> entity)
            where T : EntityModelBase
        {
            entity.ThrowIfNull();

            entity.HasKey(k => k.Id);

            entity.Property(p => p.Status)
                .HasColumnType("bit")
                .HasDefaultValueSql("((1))")
                .ValueGeneratedNever();

            entity.Property(p => p.AddingDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(p => p.UpdatingDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(p => p.AddingUserId)
                .HasColumnType("int")
                .HasDefaultValueSql("((1))");

            entity.Property(p => p.UpdatingUserId)
                .HasColumnType("int")
                .HasDefaultValueSql("((1))");
        }
    }
}