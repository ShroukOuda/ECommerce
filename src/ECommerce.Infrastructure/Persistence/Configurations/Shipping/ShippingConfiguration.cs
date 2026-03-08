using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Shipping;

public class ShippingConfiguration : IEntityTypeConfiguration<Core.Entities.Shipping.Shipping>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Shipping.Shipping> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(s => s.Method)
            .HasConversion<string>();

        builder.Property(s => s.Status)
            .HasConversion<string>();

        builder.Property(s => s.Cost)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Order)
            .WithMany(o => o.Shippings)
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.Address)
            .WithMany(a => a.Shippings)
            .HasForeignKey(s => s.AddressId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}