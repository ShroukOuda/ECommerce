using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Review;

namespace ECommerce.Domain.Entities.Review;

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
    public string UserId { get; set; }
    
    //Navigation Properties
    public virtual ICollection<ReviewHelpfulVote> ReviewHelpfulVotes { get; set; } = new List<ReviewHelpfulVote>();
    public virtual Product.Product? Product { get; set; }
    public virtual Order.Order? Order { get; set; }
    public virtual User.User? User { get; set; }
}