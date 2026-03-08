using ECommerce.Core.Entities.Product;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductVariantOptionValueConfiguration : IEntityTypeConfiguration<ProductVariantOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantOptionValue> builder)
    {
        builder.HasKey(pvov => pvov.Id);

        builder.HasIndex(pvov => new { pvov.ProductVariantId, pvov.ProductOptionValueId }).IsUnique();

        builder.HasOne(pvov => pvov.ProductVariant)
            .WithMany(pv => pv.ProductVariantOptionValues)
            .HasForeignKey(pvov => pvov.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pvov => pvov.ProductOptionValue)
            .WithMany(pov => pov.ProductVariantOptionValues)
            .HasForeignKey(pvov => pvov.ProductOptionValueId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}