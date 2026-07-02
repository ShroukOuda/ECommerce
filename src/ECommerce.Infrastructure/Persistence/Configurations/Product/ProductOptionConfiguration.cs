using ECommerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(EntityTypeBuilder<ProductOption> builder)
    {
        builder.HasKey(po => po.Id);

        builder.Property(po => po.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(po => po.DisplayType)
            .HasConversion<string>();

        builder.Property(po => po.Type)
            .HasConversion<string>();

        builder.Property(po => po.AttributeKey)
            .HasMaxLength(100);

        builder.Property(po => po.PriceValue)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(po => po.Product)
            .WithMany(p => p.ProductOptions)
            .HasForeignKey(po => po.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.ProductOptionValues)
            .WithOne(pov => pov.ProductOption)
            .HasForeignKey(pov => pov.OptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.CartItemOptions)
            .WithOne(cio => cio.ProductOption)
            .HasForeignKey(cio => cio.ProductOptionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}