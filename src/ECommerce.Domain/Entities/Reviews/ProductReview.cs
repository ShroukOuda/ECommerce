using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Review;

namespace ECommerce.Domain.Entities.Reviews;

public class ProductReview : BaseEntity<Guid>
{
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsVerifiedPurchase { get; set; } = false;
    public int HelpfulCount { get; set; } = 0;
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    
    //FK
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public virtual ICollection<ReviewHelpfulVote> ReviewHelpfulVotes { get; set; } = new List<ReviewHelpfulVote>();
    public virtual Products.Product? Product { get; set; }
    public virtual Orders.Order? Order { get; set; }
    public virtual Users.User? User { get; set; }
}