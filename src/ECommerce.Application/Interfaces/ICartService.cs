using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces;

public interface ICartService
{
    Task<GetCartDTO> GetCartByIdAsync(Guid id);
    Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId);
    Task<GetCartItemDTO> AddCartItemAsync(AddCartItemDTO dto);
    Task<GetCartItemDTO> UpdateCartItemAsync(Guid cartItemId, UpdateCartItemDTO dto);
    Task RemoveCartItemAsync(Guid cartId, Guid cartItemId);
    Task ClearCartAsync(Guid cartId);
}
