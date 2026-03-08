using ECommerce.Core.Entities.Return;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Return;

public class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
{
    public void Configure(EntityTypeBuilder<ReturnItem> builder)
    {
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ri => ri.RefundAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(ri => ri.Status)
            .HasConversion<string>();

        builder.HasOne(ri => ri.ReturnRequest)
            .WithMany(rr => rr.ReturnItems)
            .HasForeignKey(ri => ri.ReturnRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.OrderItem)
            .WithMany()
            .HasForeignKey(ri => ri.OrderItemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ri => ri.Product)
            .WithMany(p => p.ReturnItems)
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}