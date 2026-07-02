using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Order;

public class OrderConfigurations : IEntityTypeConfiguration<Domain.Entities.Orders.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Orders.Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.SubTotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.TaxAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.ShippingCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Currency)
            .HasMaxLength(10);

        builder.Property(o => o.OrderType)
            .HasConversion<string>();

        builder.Property(o => o.OrderStatus)
            .HasConversion<string>();

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.ShippingAddress)
            .WithMany(a => a.ShippingOrders)
            .HasForeignKey(o => o.ShippingAddressId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.BillingAddress)
            .WithMany(a => a.BillingOrders)
            .HasForeignKey(o => o.BillingAddressId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.OrderStatusHistories)
            .WithOne(osh => osh.Order)
            .HasForeignKey(osh => osh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.ProductReviews)
            .WithOne(pr => pr.Order)
            .HasForeignKey(pr => pr.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(o => o.CouponUsages)
            .WithOne(cu => cu.Order)
            .HasForeignKey(cu => cu.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(o => o.Shippings)
            .WithOne(s => s.Order)
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(o => o.ReturnRequests)
            .WithOne(rr => rr.Order)
            .HasForeignKey(rr => rr.OrderId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}