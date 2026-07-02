using ECommerce.Domain.Entities.Inventories;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Inventory;

public class InventoryHistoryConfiguration : IEntityTypeConfiguration<InventoryHistory>
{
    public void Configure(EntityTypeBuilder<InventoryHistory> builder)
    {
        builder.HasKey(ih => ih.Id);

        builder.Property(ih => ih.ChangeType)
            .HasConversion<string>();

        builder.Property(ih => ih.ReferencedId)
            .HasMaxLength(100);

        builder.Property(ih => ih.ReferencedType)
            .HasMaxLength(100);

        builder.Property(ih => ih.Notes)
            .HasMaxLength(500);

        builder.HasOne(ih => ih.Product)
            .WithMany()
            .HasForeignKey(ih => ih.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ih => ih.ProductVariant)
            .WithMany(pv => pv.InventoryHistories)
            .HasForeignKey(ih => ih.ProductVariantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ih => ih.User)
            .WithMany(u => u.InventoryHistories)
            .HasForeignKey(ih => ih.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}