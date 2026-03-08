using ECommerce.Core.Entities.Review;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Review;

public class ReviewHelpfulVoteConfiguration : IEntityTypeConfiguration<ReviewHelpfulVote>
{
    public void Configure(EntityTypeBuilder<ReviewHelpfulVote> builder)
    {
        builder.HasKey(rhv => rhv.Id);

        builder.HasOne(rhv => rhv.ProductReview)
            .WithMany(pr => pr.ReviewHelpfulVotes)
            .HasForeignKey(rhv => rhv.ProductReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}