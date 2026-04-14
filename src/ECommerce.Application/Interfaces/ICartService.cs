using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces;

public interface ICartService
{
    Task<GetCartDTO> GetCartByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddCartItemAsync(AddCartItemDTO dto, CancellationToken ct = default);
    Task UpdateCartItemAsync(UpdateCartItemDTO dto, CancellationToken ct = default);
    Task RemoveCartItemAsync(Guid cartItemId, CancellationToken ct = default);
    Task ClearCartAsync(Guid cartId, CancellationToken ct = default);
}
