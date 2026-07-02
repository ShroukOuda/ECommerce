using System.Text.Json;
using ECommerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.Id);

        builder.HasIndex(pv => pv.Sku).IsUnique();

        builder.Property(pv => pv.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pv => pv.VariantName)
            .HasMaxLength(200);

        var dictionaryComparer = new ValueComparer<Dictionary<string, string>>(
            (d1, d2) => JsonSerializer.Serialize(d1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(d2, (JsonSerializerOptions?)null),
            d => d == null ? 0 : JsonSerializer.Serialize(d, (JsonSerializerOptions?)null).GetHashCode(),
            d => JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(d, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!);

        builder.Property(pv => pv.Attributes)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null))
            .Metadata.SetValueComparer(dictionaryComparer);

        builder.Property(pv => pv.Size)
            .HasMaxLength(100);

        builder.Property(pv => pv.Color)
            .HasMaxLength(100);

        builder.Property(pv => pv.Material)
            .HasMaxLength(100);

        builder.Property(pv => pv.PriceAdjustment)
            .HasColumnType("decimal(18,2)");

        builder.Property(pv => pv.StockStatus)
            .HasConversion<string>();

        builder.Property(pv => pv.Status)
            .HasConversion<string>();

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pv => pv.ProductImages)
            .WithOne(pi => pi.ProductVariant)
            .HasForeignKey(pi => pi.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(pv => pv.ProductVariantOptionValues)
            .WithOne(pvov => pvov.ProductVariant)
            .HasForeignKey(pvov => pvov.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pv => pv.OrderItems)
            .WithOne(oi => oi.ProductVariant)
            .HasForeignKey(oi => oi.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(pv => pv.CartItems)
            .WithOne(ci => ci.ProductVariant)
            .HasForeignKey(ci => ci.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(pv => pv.InventoryHistories)
            .WithOne(ih => ih.ProductVariant)
            .HasForeignKey(ih => ih.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}