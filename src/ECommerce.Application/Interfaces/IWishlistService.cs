using ECommerce.Application.DTO.Wishlist;

namespace ECommerce.Application.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<GetWishlistDTO>> GetWishlistByUserIdAsync(string userId);
    Task<GetWishlistDTO> AddToWishlistAsync(AddWishlistDTO dto);
    Task RemoveFromWishlistAsync(Guid id);
}
