using ECommerce.Core.Entities.Cart;
using ECommerce.Core.Enums.Cart;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class CartSeed
{
    private static readonly DateTime BaseDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedCarts(ModelBuilder modelBuilder)
    {
        var carts = new List<Cart>();
        var cartItems = new List<CartItem>();
        var cartItemOptions = new List<CartItemOption>();
        int cartItemId = 1;
        int cartItemOptionId = 1;

        // 50 active carts, users 50-99
        for (int cartId = 1; cartId <= 50; cartId++)
        {
            var userIndex = 49 + cartId;
            var userId = UserSeed.GetUserId(userIndex);
            var createdAt = BaseDate.AddDays(-cartId).AddHours((cartId * 3) % 24);
            var itemCount = (cartId % 6) + 1; // 1-6 items

            carts.Add(new Cart
            {
                Id = cartId,
                UserId = userId,
                GuestToken = string.Empty,
                DiscountAmount = 0,
                ExpiresAt = createdAt.AddDays(30),
                Status = CartStatus.Active,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                IsDeleted = false
            });

            for (int item = 0; item < itemCount; item++)
            {
                var productId = ((cartId * 5 + item * 13) % 80) + 1;
                var quantity = (item % 3) + 1;
                // Price from product seed
                decimal price = productId switch
                {
                    1 => 1299.99m, 2 => 799.99m, 3 => 1199.99m, 4 => 799.99m, 5 => 449.99m,
                    <= 10 => 599.99m, <= 20 => 199.99m, <= 35 => 899.99m, <= 43 => 449.99m,
                    <= 53 => 179.99m, <= 61 => 799.99m, <= 69 => 999.99m, <= 76 => 299.99m,
                    _ => 59.99m
                };

                var variantId = ((productId - 1) * 3 + 1);
                if (variantId > 500) variantId = 1;

                cartItems.Add(new CartItem
                {
                    Id = cartItemId,
                    CartId = cartId,
                    ProductId = productId,
                    VariantId = variantId,
                    Quantity = quantity,
                    Price = price,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsDeleted = false
                });

                // One option per cart item
                cartItemOptions.Add(new CartItemOption
                {
                    Id = cartItemOptionId++,
                    CartItemId = cartItemId,
                    ProductOptionId = ((productId - 1) * 2 + 1), // approximate first option ID
                    OptionName = "Color",
                    OptionValue = item % 2 == 0 ? "Black" : "White",
                    PriceAdjustment = 0,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsDeleted = false
                });

                cartItemId++;
            }
        }

        modelBuilder.Entity<Cart>().HasData(carts.ToArray());
        modelBuilder.Entity<CartItem>().HasData(cartItems.ToArray());
        modelBuilder.Entity<CartItemOption>().HasData(cartItemOptions.ToArray());
    }
}
