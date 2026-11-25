using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Shipping;

public class ShippingConfiguration : IEntityTypeConfiguration<Domain.Entities.Shippings.Shipping>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Shippings.Shipping> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(s => s.Method)
            .HasConversion<string>();

        // Bounded so the filtered unique index below can use it in its
        // predicate against a normal-width column. The longest
        // ShippingStatus name is OutForDelivery (14 characters).
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Cost)
            .HasColumnType("decimal(18,2)");

        // One active (non-Failed) shipment per order; the whole order
        // ships together. Enforced by the database so concurrent
        // creates cannot both succeed.
        builder.HasIndex(s => s.OrderId)
            .IsUnique()
            .HasFilter("[Status] <> 'Failed'");

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
