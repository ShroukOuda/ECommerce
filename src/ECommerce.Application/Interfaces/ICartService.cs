using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces;

public interface ICartService
{
    Task<GetCartDTO> GetCartByIdAsync(int id, CancellationToken ct = default);
    Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddCartItemAsync(AddCartItemDTO dto, CancellationToken ct = default);
    Task UpdateCartItemAsync(UpdateCartItemDTO dto, CancellationToken ct = default);
    Task RemoveCartItemAsync(int cartItemId, CancellationToken ct = default);
    Task ClearCartAsync(int cartId, CancellationToken ct = default);
}
