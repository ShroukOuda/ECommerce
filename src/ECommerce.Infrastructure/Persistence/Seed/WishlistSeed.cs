using ECommerce.Core.Entities.Wishlist;
using ECommerce.Core.Enums.Wishlist;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class WishlistSeed
{
    private static readonly DateTime BaseDate = new(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedWishlists(ModelBuilder modelBuilder)
    {
        var wishlists = new List<Wishlist>();
        int wishlistId = 1;

        // 150 users (users 20-169) each have 1-5 wishlist items = ~300 entries
        for (int userOffset = 0; userOffset < 150; userOffset++)
        {
            var userIndex = 20 + userOffset;
            var userId = UserSeed.GetUserId(userIndex);
            var itemCount = (userOffset % 5) + 1; // 1-5 items

            for (int item = 0; item < itemCount; item++)
            {
                var productId = ((userOffset * 3 + item * 11) % 80) + 1;
                var createdAt = BaseDate.AddDays((wishlistId * 2) % 365).AddHours((wishlistId * 5) % 24);

                wishlists.Add(new Wishlist
                {
                    Id = wishlistId,
                    UserId = userId,
                    ProductId = productId,
                    Status = WishlistStatus.Active,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsDeleted = false
                });
                wishlistId++;
            }
        }

        modelBuilder.Entity<Wishlist>().HasData(wishlists.ToArray());
    }
}
