using ECommerce.Core.Entities.Review;
using ECommerce.Core.Enums.Review;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ReviewSeed
{
    private static readonly DateTime BaseDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Realistic review titles (mix of English and Arabic)
    private static readonly string[] ReviewTitles =
    {
        "Excellent product, highly recommended!",
        "Great value for money",
        "Exceeded my expectations",
        "Works perfectly, very satisfied",
        "Amazing quality and fast shipping",
        "Best purchase I've made this year",
        "Good product but could be better",
        "Decent quality for the price",
        "Not bad, but not great either",
        "Average product, nothing special",
        "Disappointed with the quality",
        "Not worth the price",
        "Product arrived damaged",
        "Terrible experience, would not recommend",
        "منتج ممتاز، أنصح به بشدة",
        "جودة عالية وسعر مناسب",
        "أفضل شراء قمت به",
        "منتج جيد جداً",
        "جودة ممتازة وتوصيل سريع",
        "سعر مميز وأداء رائع",
        "Superb build quality",
        "Fast delivery, great packaging",
        "Perfect for everyday use",
        "Love this product!",
        "Solid performance, no complaints",
        "Very happy with my purchase",
        "Could be improved but still good",
        "Just what I needed",
        "Impressive features for the price",
        "Top-notch quality",
        "Beautiful design and functionality",
        "Reliable and well-made",
        "Good but shipping was slow",
        "Premium quality product",
        "Fantastic! Will buy again",
        "رائع! سأشتري مرة أخرى",
        "منتج عملي ومفيد",
        "الأفضل في فئته",
        "يستحق كل ريال",
        "تجربة شراء ممتازة",
        "Outstanding quality",
        "Exactly as described",
        "Better than expected",
        "Great gift idea",
        "Worth every penny",
        "Five stars! Absolutely love it",
        "Smooth and seamless experience",
        "Very pleased with the purchase",
        "Meets all my needs perfectly",
        "A must-have product",
    };

    public static void SeedReviews(ModelBuilder modelBuilder)
    {
        var reviews = new List<ProductReview>();
        var helpfulVotes = new List<ReviewHelpfulVote>();
        int reviewId = 1;
        int voteId = 1;

        // Generate reviews for all 80 products
        for (int productId = 1; productId <= 80; productId++)
        {
            // Each product gets between 10 and 20 reviews deterministically
            var reviewCount = 10 + (productId % 11); // 10-20

            for (int r = 0; r < reviewCount; r++)
            {
                // Assign to different users (customers: users 16-200)
                var userIndex = 16 + ((productId * 7 + r * 13) % 185);
                var userId = UserSeed.GetUserId(userIndex);

                // Rating distribution: mostly 4-5 stars
                int rating;
                var ratingIndex = (reviewId * 31) % 100;
                if (ratingIndex < 35) rating = 5;         // 35% five stars
                else if (ratingIndex < 70) rating = 4;    // 35% four stars
                else if (ratingIndex < 85) rating = 3;    // 15% three stars
                else if (ratingIndex < 95) rating = 2;    // 10% two stars
                else rating = 1;                           // 5% one star

                // Status distribution: 90% Approved, 8% Pending, 2% Rejected
                ReviewStatus status;
                var statusIndex = reviewId % 100;
                if (statusIndex < 90) status = ReviewStatus.Approved;
                else if (statusIndex < 98) status = ReviewStatus.Pending;
                else status = ReviewStatus.Rejected;

                // Title based on rating
                int titleIndex;
                if (rating >= 4) titleIndex = (reviewId * 3) % 30; // first 30 are positive
                else if (rating == 3) titleIndex = 6 + ((reviewId * 5) % 8); // mixed titles
                else titleIndex = 10 + ((reviewId * 2) % 4); // last are negative

                var title = ReviewTitles[titleIndex % ReviewTitles.Length];

                // Deterministic date
                var dayOffset = (reviewId * 11) % 548;
                var hourOffset = (reviewId * 5) % 24;
                var createdAt = BaseDate.AddDays(-548 + dayOffset).AddHours(hourOffset);

                // Helpful count
                var helpfulCount = (reviewId * 17) % 31; // 0-30

                // Assign to an order that the user placed (approximately)
                // Orders are 1-500, we'll pick one deterministically
                var orderId = ((userIndex - 16) * 3 + r) % 500 + 1;

                reviews.Add(new ProductReview
                {
                    Id = reviewId,
                    ProductId = productId,
                    UserId = userId,
                    OrderId = orderId,
                    Rating = rating,
                    Title = title,
                    IsVerifiedPurchase = status == ReviewStatus.Approved && (reviewId % 4 != 0),
                    HelpfulCount = helpfulCount,
                    Status = status,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsDeleted = false
                });

                // Generate 0-5 helpful votes per review
                var voteCount = helpfulCount > 0 ? Math.Min(helpfulCount, 5) : 0;
                for (int v = 0; v < voteCount; v++)
                {
                    var voterIndex = 16 + ((reviewId * 11 + v * 7) % 185);
                    helpfulVotes.Add(new ReviewHelpfulVote
                    {
                        Id = voteId++,
                        ProductReviewId = reviewId,
                        IsHelpful = (v % 5 != 0), // 80% helpful
                        CreatedAt = createdAt.AddDays(v + 1),
                        UpdatedAt = createdAt.AddDays(v + 1),
                        IsDeleted = false
                    });
                }

                reviewId++;
            }
        }

        modelBuilder.Entity<ProductReview>().HasData(reviews.ToArray());
        modelBuilder.Entity<ReviewHelpfulVote>().HasData(helpfulVotes.ToArray());
    }
}
