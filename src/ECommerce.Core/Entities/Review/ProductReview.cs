namespace ECommerce.Core.Entities.Review;

public class ProductReview : BaseEntity<int>
{
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsVerifiedPurchase { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public int HelpfulCount { get; set; } = 0;
    
    //FK
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    
    //Navigation Properties
    public virtual ICollection<ReviewHelpfulVote> ReviewHelpfulVotes { get; set; } = new List<ReviewHelpfulVote>();
    public Product.Product? Product { get; set; }
    public Order.Order? Order { get; set; }
}