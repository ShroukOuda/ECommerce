using ECommerce.Core.Entities.Review;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Review;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Title)
            .HasMaxLength(200);

        builder.Property(pr => pr.Status)
            .HasConversion<string>();

        builder.HasOne(pr => pr.Product)
            .WithMany(p => p.ProductReviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(pr => pr.Order)
            .WithMany(o => o.ProductReviews)
            .HasForeignKey(pr => pr.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(pr => pr.User)
            .WithMany(u => u.ProductReviews)
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(pr => pr.ReviewHelpfulVotes)
            .WithOne(rhv => rhv.ProductReview)
            .HasForeignKey(rhv => rhv.ProductReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}