using ECommerce.Core.Entities.Coupon;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class CouponUsageSeed
{
    private static readonly DateTime BaseDate = new(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    // Active coupon IDs from CouponSeed: 1-10
    // WELCOME10(1), SAVE20(2), ELECTRONICS15(3), FLASH50(4), SUMMER25(5),
    // FREESHIP(6), VIP30(7), BACK2SCHOOL(8), NEWUSER15(9), WEEKEND10(10)

    public static void SeedCouponUsages(ModelBuilder modelBuilder)
    {
        var usages = new List<CouponUsage>();
        int usageId = 1;

        // ~30% of 500 orders used coupons = orders where orderId % 3 == 0 → ~166 orders
        for (int orderId = 3; orderId <= 500; orderId += 3)
        {
            var userIndex = ((orderId - 1) % 180) + 16;
            var userId = UserSeed.GetUserId(userIndex);
            var orderDate = BaseDate.AddDays((orderId * 33) % 540).AddHours((orderId * 7) % 24);

            // Rotate through the 10 active coupons
            var couponId = ((orderId / 3 - 1) % 10) + 1;

            // Discount amounts based on coupon type
            decimal discountAmount = couponId switch
            {
                1 => 25.00m,   // WELCOME10 - 10%
                2 => 50.00m,   // SAVE20 - 20%
                3 => 37.50m,   // ELECTRONICS15 - 15%
                4 => 50.00m,   // FLASH50 - $50 flat
                5 => 62.50m,   // SUMMER25 - 25%
                6 => 9.99m,    // FREESHIP - free shipping
                7 => 75.00m,   // VIP30 - 30%
                8 => 30.00m,   // BACK2SCHOOL - 15%
                9 => 37.50m,   // NEWUSER15 - 15%
                10 => 25.00m,  // WEEKEND10 - 10%
                _ => 10.00m
            };

            usages.Add(new CouponUsage
            {
                Id = usageId++,
                CouponId = couponId,
                OrderId = orderId,
                UserId = userId,
                DiscountAmount = discountAmount,
                CreatedAt = orderDate,
                UpdatedAt = orderDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<CouponUsage>().HasData(usages.ToArray());
    }
}
