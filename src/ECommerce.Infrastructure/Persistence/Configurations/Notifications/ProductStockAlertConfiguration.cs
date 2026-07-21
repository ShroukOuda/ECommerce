using ECommerce.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Notifications;

public class ProductStockAlertConfiguration
    : IEntityTypeConfiguration<ProductStockAlert>
{
    public void Configure(EntityTypeBuilder<ProductStockAlert> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsNotified)
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => new
        {
            x.UserId,
            x.ProductId
        }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}