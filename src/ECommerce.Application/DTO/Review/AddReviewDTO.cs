namespace ECommerce.Application.DTO.Review;

public class AddReviewDTO
{
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
