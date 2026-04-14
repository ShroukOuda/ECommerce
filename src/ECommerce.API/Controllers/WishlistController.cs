using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class WishlistController : BaseController
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var items = await _wishlistService.GetWishlistByUserIdAsync(userId);
        return Ok(items);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddWishlistDTO dto)
    {
        await _wishlistService.AddToWishlistAsync(dto);
        return Ok(new ResponseAPI(200, "Added to wishlist successfully"));
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _wishlistService.RemoveFromWishlistAsync(id);
        return Ok(new ResponseAPI(200, "Removed from wishlist successfully"));
    }
}
