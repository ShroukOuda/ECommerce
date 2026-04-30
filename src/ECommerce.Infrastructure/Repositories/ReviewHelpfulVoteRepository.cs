using ECommerce.Domain.Entities.Review;

namespace ECommerce.Infrastructure.Repositories;

public class ReviewHelpfulVoteRepository : GenericRepository<ReviewHelpfulVote, Guid>, IReviewHelpfulVoteRepository
{
    public ReviewHelpfulVoteRepository(AppDbContext context) : base(context) { }
}
