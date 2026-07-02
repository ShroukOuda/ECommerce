using ECommerce.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Order;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.HasKey(osh => osh.Id);

        builder.Property(osh => osh.OrderStatus)
            .HasConversion<string>();

        builder.HasOne(osh => osh.Order)
            .WithMany(o => o.OrderStatusHistories)
            .HasForeignKey(osh => osh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}