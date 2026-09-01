using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces;

public interface ICartService
{
    Task<GetCartDTO> GetCartByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GetCartItemDTO> AddCartItemAsync(AddCartItemDTO dto, CancellationToken ct = default);
    Task<GetCartItemDTO> UpdateCartItemAsync(Guid cartItemId, UpdateCartItemDTO dto, CancellationToken ct = default);
    Task RemoveCartItemAsync(Guid cartId, Guid cartItemId, CancellationToken ct = default);
    Task ClearCartAsync(Guid cartId, CancellationToken ct = default);
}
