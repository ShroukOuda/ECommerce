using ECommerce.Core.Entities.Coupon;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Coupon;

public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> builder)
    {
        builder.HasKey(cu => cu.Id);

        builder.Property(cu => cu.DiscountAmount)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(cu => cu.Coupon)
            .WithMany(c => c.CouponUsages)
            .HasForeignKey(cu => cu.CouponId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(cu => cu.Order)
            .WithMany(o => o.CouponUsages)
            .HasForeignKey(cu => cu.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(cu => cu.User)
            .WithMany(u => u.CouponUsages)
            .HasForeignKey(cu => cu.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}