using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Coupon;

public class CouponConfiguration : IEntityTypeConfiguration<Core.Entities.Coupon.Coupon>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Coupon.Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.DiscountType)
            .HasConversion<string>();

        builder.Property(c => c.Status)
            .HasConversion<string>();

        builder.Property(c => c.DiscountValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.MinPurchaseAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.MaxDiscountAmount)
            .HasColumnType("decimal(18,2)");

        builder.HasMany(c => c.CouponUsages)
            .WithOne(cu => cu.Coupon)
            .HasForeignKey(cu => cu.CouponId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}