using ECommerce.Core.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.User;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.AddressLine2)
            .HasMaxLength(200);

        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Type)
            .HasConversion<string>();

        builder.Property(a => a.Status)
            .HasConversion<string>();

        builder.HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.ShippingOrders)
            .WithOne(o => o.ShippingAddress)
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(a => a.BillingOrders)
            .WithOne(o => o.BillingAddress)
            .HasForeignKey(o => o.BillingAddressId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(a => a.Shippings)
            .WithOne(s => s.Address)
            .HasForeignKey(s => s.AddressId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}