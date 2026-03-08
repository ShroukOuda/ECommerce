using ECommerce.Core.Entities.Coupon;
using ECommerce.Core.Enums.Coupon;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class CouponSeed
{
    private static readonly DateTime CreatedAt = new(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedCoupons(ModelBuilder modelBuilder)
    {
        var coupons = new List<Coupon>
        {
            // ===== ACTIVE COUPONS (10) =====
            new()
            {
                Id = 1, Code = "WELCOME10", Description = "10% off for new users, minimum order $50",
                DiscountType = DiscountType.Percentage, DiscountValue = 10, MinPurchaseAmount = 50,
                MaxDiscountAmount = 20, UsageLimit = 10000, UsedCount = 1250, PerUserLimit = 1,
                ValidFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 2, Code = "SAVE20", Description = "20% off on orders above $100",
                DiscountType = DiscountType.Percentage, DiscountValue = 20, MinPurchaseAmount = 100,
                MaxDiscountAmount = 50, UsageLimit = 5000, UsedCount = 800, PerUserLimit = 3,
                ValidFrom = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 3, Code = "ELECTRONICS15", Description = "15% off all electronics",
                DiscountType = DiscountType.Percentage, DiscountValue = 15, MinPurchaseAmount = 0,
                MaxDiscountAmount = 100, UsageLimit = 3000, UsedCount = 450, PerUserLimit = 2,
                ValidFrom = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 4, Code = "FLASH50", Description = "$50 off on orders above $200, limited to 100 uses",
                DiscountType = DiscountType.Fixed, DiscountValue = 50, MinPurchaseAmount = 200,
                MaxDiscountAmount = 50, UsageLimit = 100, UsedCount = 67, PerUserLimit = 1,
                ValidFrom = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 5, Code = "SUMMER25", Description = "25% off clothing and fashion",
                DiscountType = DiscountType.Percentage, DiscountValue = 25, MinPurchaseAmount = 0,
                MaxDiscountAmount = 75, UsageLimit = 2000, UsedCount = 320, PerUserLimit = 2,
                ValidFrom = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 9, 30, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 6, Code = "FREESHIP", Description = "Free shipping on any order",
                DiscountType = DiscountType.FreeShipping, DiscountValue = 0, MinPurchaseAmount = 0,
                MaxDiscountAmount = 30, UsageLimit = 5000, UsedCount = 1100, PerUserLimit = 5,
                ValidFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 7, Code = "VIP30", Description = "30% off for VIP customers",
                DiscountType = DiscountType.Percentage, DiscountValue = 30, MinPurchaseAmount = 150,
                MaxDiscountAmount = 100, UsageLimit = 500, UsedCount = 85, PerUserLimit = 2,
                ValidFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 8, Code = "BACK2SCHOOL", Description = "20% off books and electronics for back to school",
                DiscountType = DiscountType.Percentage, DiscountValue = 20, MinPurchaseAmount = 50,
                MaxDiscountAmount = 40, UsageLimit = 3000, UsedCount = 600, PerUserLimit = 2,
                ValidFrom = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 10, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 9, Code = "NEWUSER15", Description = "15% off first order for new users",
                DiscountType = DiscountType.Percentage, DiscountValue = 15, MinPurchaseAmount = 0,
                MaxDiscountAmount = 30, UsageLimit = 10000, UsedCount = 2000, PerUserLimit = 1,
                ValidFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 10, Code = "WEEKEND10", Description = "10% off on weekends only",
                DiscountType = DiscountType.Percentage, DiscountValue = 10, MinPurchaseAmount = 30,
                MaxDiscountAmount = 15, UsageLimit = 8000, UsedCount = 1500, PerUserLimit = 5,
                ValidFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Active, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },

            // ===== EXPIRED / INACTIVE COUPONS (10) =====
            new()
            {
                Id = 11, Code = "BF2023", Description = "Black Friday 2023 - 40% off everything",
                DiscountType = DiscountType.Percentage, DiscountValue = 40, MinPurchaseAmount = 100,
                MaxDiscountAmount = 200, UsageLimit = 5000, UsedCount = 4800, PerUserLimit = 1,
                ValidFrom = new DateTime(2023, 11, 24, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2023, 11, 27, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 12, Code = "CYBER2023", Description = "Cyber Monday 2023 - $30 off",
                DiscountType = DiscountType.Fixed, DiscountValue = 30, MinPurchaseAmount = 75,
                MaxDiscountAmount = 30, UsageLimit = 3000, UsedCount = 2800, PerUserLimit = 1,
                ValidFrom = new DateTime(2023, 11, 27, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2023, 11, 28, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 13, Code = "XMAS2023", Description = "Christmas 2023 - 20% off gifts",
                DiscountType = DiscountType.Percentage, DiscountValue = 20, MinPurchaseAmount = 50,
                MaxDiscountAmount = 50, UsageLimit = 4000, UsedCount = 3200, PerUserLimit = 2,
                ValidFrom = new DateTime(2023, 12, 15, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2023, 12, 26, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 14, Code = "NY2024", Description = "New Year 2024 - 15% off",
                DiscountType = DiscountType.Percentage, DiscountValue = 15, MinPurchaseAmount = 0,
                MaxDiscountAmount = 25, UsageLimit = 2000, UsedCount = 1800, PerUserLimit = 1,
                ValidFrom = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2024, 1, 3, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 15, Code = "SPRING2024", Description = "Spring sale 2024 - 25% off clothing",
                DiscountType = DiscountType.Percentage, DiscountValue = 25, MinPurchaseAmount = 40,
                MaxDiscountAmount = 40, UsageLimit = 2000, UsedCount = 1600, PerUserLimit = 2,
                ValidFrom = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2024, 4, 15, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 16, Code = "RAMADAN2024", Description = "Ramadan deal - 20% off",
                DiscountType = DiscountType.Percentage, DiscountValue = 20, MinPurchaseAmount = 30,
                MaxDiscountAmount = 35, UsageLimit = 3000, UsedCount = 2500, PerUserLimit = 2,
                ValidFrom = new DateTime(2024, 3, 11, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2024, 4, 10, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 17, Code = "SUMMER2023", Description = "Summer 2023 clearance - 30% off",
                DiscountType = DiscountType.Percentage, DiscountValue = 30, MinPurchaseAmount = 60,
                MaxDiscountAmount = 60, UsageLimit = 1500, UsedCount = 1500, PerUserLimit = 1,
                ValidFrom = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2023, 8, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.UsageLimitReached, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 18, Code = "LAUNCH10", Description = "Launch promotion - $10 off",
                DiscountType = DiscountType.Fixed, DiscountValue = 10, MinPurchaseAmount = 25,
                MaxDiscountAmount = 10, UsageLimit = 500, UsedCount = 500, PerUserLimit = 1,
                ValidFrom = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2023, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.UsageLimitReached, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 19, Code = "TESTCOUPON", Description = "Test coupon - disabled",
                DiscountType = DiscountType.Percentage, DiscountValue = 5, MinPurchaseAmount = 0,
                MaxDiscountAmount = 5, UsageLimit = 100, UsedCount = 10, PerUserLimit = 1,
                ValidFrom = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Disabled, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
            new()
            {
                Id = 20, Code = "VALENTINE24", Description = "Valentine's Day 2024 - 15% off",
                DiscountType = DiscountType.Percentage, DiscountValue = 15, MinPurchaseAmount = 50,
                MaxDiscountAmount = 30, UsageLimit = 2000, UsedCount = 1700, PerUserLimit = 1,
                ValidFrom = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                ValidUntil = new DateTime(2024, 2, 15, 23, 59, 59, DateTimeKind.Utc),
                Status = CouponStatus.Expired, CreatedAt = CreatedAt, UpdatedAt = CreatedAt, IsDeleted = false
            },
        };

        modelBuilder.Entity<Coupon>().HasData(coupons.ToArray());
    }
}
