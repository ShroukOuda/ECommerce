using ECommerce.Application.DTO.Wishlist;

namespace ECommerce.Application.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<GetWishlistDTO>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GetWishlistDTO> AddToWishlistAsync(AddWishlistDTO dto, CancellationToken ct = default);
    Task RemoveFromWishlistAsync(Guid id, CancellationToken ct = default);
}
