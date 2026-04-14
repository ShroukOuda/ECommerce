namespace ECommerce.Application.DTO.Cart;

public class GetCartDTO
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime ExpiresAt { get; set; }
    public List<GetCartItemDTO> Items { get; set; } = new();
}
