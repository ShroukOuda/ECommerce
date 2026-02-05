namespace ECommerce.Core.Entities.Review;

public class ReviewHelpfulVote : BaseEntity<int>
{
    public bool IsHelpful { get; set; }
    
    //FK
    public int ProductReviewId { get; set; }
    
    //Navigation Properties
    public virtual ProductReview? ProductReview { get; set; }
}