namespace ECommerce.Application.DTO.Wishlist;

public class AddWishlistDTO
{
    public Guid ProductId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
