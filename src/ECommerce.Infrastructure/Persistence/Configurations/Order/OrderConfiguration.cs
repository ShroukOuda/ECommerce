using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Order;

public class OrderConfigurations : IEntityTypeConfiguration<Core.Entities.Order.Order>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Order.Order> builder)
    {
        builder.HasIndex(o => o.OrderNumber).IsUnique();
    }
}