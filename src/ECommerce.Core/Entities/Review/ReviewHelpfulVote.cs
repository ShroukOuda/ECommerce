namespace ECommerce.Core.Entities.Review;

public class ReviewHelpfulVote : BaseEntity<Guid>
{
    public bool IsHelpful { get; set; }
    
    //FK
    public Guid ProductReviewId { get; set; }
    
    //Navigation Properties
    public virtual ProductReview? ProductReview { get; set; }
}