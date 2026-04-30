using ECommerce.Domain.Entities.Product;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductOptionValueConfiguration : IEntityTypeConfiguration<ProductOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductOptionValue> builder)
    {
        builder.HasKey(pov => pov.Id);

        builder.Property(pov => pov.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pov => pov.Label)
            .HasMaxLength(200);

        builder.Property(pov => pov.PriceValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(pov => pov.ImageUrl)
            .HasMaxLength(2048);

        builder.HasOne(pov => pov.ProductOption)
            .WithMany(po => po.ProductOptionValues)
            .HasForeignKey(pov => pov.OptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pov => pov.ProductVariantOptionValues)
            .WithOne(pvov => pvov.ProductOptionValue)
            .HasForeignKey(pvov => pvov.ProductOptionValueId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}