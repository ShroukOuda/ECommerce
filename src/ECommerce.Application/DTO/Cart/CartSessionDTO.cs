namespace ECommerce.Application.DTO.Cart;
public class CartSessionDTO
{
    public Guid CartId { get; set; }
    // Returned once. Caller must treat it as a bearer capability, never a URL parameter.
    public string? GuestToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
