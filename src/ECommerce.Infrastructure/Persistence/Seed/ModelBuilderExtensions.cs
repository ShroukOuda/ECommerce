namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // Identity & Users (no FK dependencies)
        RoleSeed.SeedRoles(modelBuilder);
        UserSeed.SeedUsers(modelBuilder);

        // Addresses (depends on Users)
        AddressSeed.SeedAddresses(modelBuilder);

        // Brands (no FK dependencies)
        BrandSeed.SeedBrands(modelBuilder);

        // Categories (no FK dependencies, self-referencing)
        CategorySeed.SeedCategories(modelBuilder);

        // Products (depends on Categories, Brands)
        ProductSeed.SeedProducts(modelBuilder);
        ProductImageSeed.SeedProductImages(modelBuilder);

        // Product Options & Variants (depends on Products)
        ProductOptionSeed.SeedProductOptions(modelBuilder);
        ProductVariantSeed.SeedProductVariants(modelBuilder);

        // Coupons (no FK dependencies)
        CouponSeed.SeedCoupons(modelBuilder);

        // Orders (depends on Users, Addresses, Coupons)
        OrderSeed.SeedOrders(modelBuilder);

        // Coupon Usage (depends on Coupons, Orders, Users)
        CouponUsageSeed.SeedCouponUsages(modelBuilder);

        // Payments (depends on Orders, Users)
        PaymentSeed.SeedPayments(modelBuilder);

        // Shipping (depends on Orders, Addresses)
        ShippingSeed.SeedShippings(modelBuilder);

        // Returns (depends on Orders, Users, OrderItems, Products)
        ReturnSeed.SeedReturns(modelBuilder);

        // Reviews (depends on Products, Users)
        ReviewSeed.SeedReviews(modelBuilder);

        // Wishlists (depends on Products, Users)
        WishlistSeed.SeedWishlists(modelBuilder);

        // Carts (depends on Products, Users, Variants)
        CartSeed.SeedCarts(modelBuilder);

        // Inventory History (depends on Products, Users)
        InventoryHistorySeed.SeedInventoryHistory(modelBuilder);
    }
}