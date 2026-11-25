using ECommerce.Domain.Entities.Shippings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Shipping;

public class ShippingStatusHistoryConfiguration : IEntityTypeConfiguration<ShippingStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShippingStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.ShippingStatus)
            .HasConversion<string>();

        builder.Property(h => h.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.Reason)
            .HasMaxLength(500);

        builder.HasOne(h => h.Shipping)
            .WithMany(s => s.StatusHistories)
            .HasForeignKey(h => h.ShippingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
