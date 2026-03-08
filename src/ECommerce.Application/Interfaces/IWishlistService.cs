using ECommerce.Application.DTO.Wishlist;

namespace ECommerce.Application.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<GetWishlistDTO>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddToWishlistAsync(AddWishlistDTO dto, CancellationToken ct = default);
    Task RemoveFromWishlistAsync(int id, CancellationToken ct = default);
}
