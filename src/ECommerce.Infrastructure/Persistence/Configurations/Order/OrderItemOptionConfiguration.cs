using ECommerce.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Order;

public class OrderItemOptionConfiguration : IEntityTypeConfiguration<OrderItemOption>
{
    public void Configure(EntityTypeBuilder<OrderItemOption> builder)
    {
        builder.HasKey(oio => oio.Id);

        builder.Property(oio => oio.OptionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oio => oio.OptionValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(oio => oio.PriceAdjustment)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(oio => oio.OrderItem)
            .WithMany(oi => oi.OrderItemOptions)
            .HasForeignKey(oio => oio.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}