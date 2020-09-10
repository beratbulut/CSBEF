using CSBEF.Core.Models;
using CSBEF.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSBEF.Core.Helpers
{
    /// <summary>
    /// TODO: To be translated into English
    /// Entity modellerin mapper sınıfları için oluşturulmuş bir helper sınıftır.
    /// "IEntityModelBase" içerisindeki varsayılan kolonlar için tüm mapper sınıflarda aynı tanımlar tekrar etmek zorundadır.
    /// Bu işi kolaylaştırmak için bu helper kullanılabilir. Böylece bu her tabloda olan ön tanımlı kolonlar, mapper içerisine entegre edilmiş olur.
    /// </summary>
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