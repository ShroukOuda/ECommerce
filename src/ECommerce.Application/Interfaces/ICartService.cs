using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces;

public interface ICartService
{
    Task<GetCartDTO> GetCartByIdAsync(Guid id, string? userId = null, string? guestToken = null);
    Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId);
    Task<GetCartItemDTO> AddCartItemAsync(AddCartItemDTO dto, string? userId = null, string? guestToken = null);
    Task<GetCartItemDTO> UpdateCartItemAsync(Guid cartItemId, UpdateCartItemDTO dto, string? userId = null, string? guestToken = null);
    Task RemoveCartItemAsync(Guid cartId, Guid cartItemId, string? userId = null, string? guestToken = null);
    Task ClearCartAsync(Guid cartId, string? userId = null, string? guestToken = null);
    Task<CartSessionDTO> CreateCartAsync(string? userId);
    Task<GetCartDTO> ClaimGuestCartAsync(Guid cartId, string guestToken, string userId);
}
