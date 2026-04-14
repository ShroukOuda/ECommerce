namespace ECommerce.Application.DTO.Review;

public class GetReviewDTO
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsVerifiedPurchase { get; set; }
    public int HelpfulCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
