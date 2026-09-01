using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

[Authorize(Roles = "Customer")]
public class WishlistController : BaseController
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private string CurrentUserId => User?.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet()]
    public async Task<IActionResult> GetMyWishlist()
    {
        var items = await _wishlistService.GetWishlistByUserIdAsync(CurrentUserId);
        return Success(
            items,
            "Wishlist retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> Add(AddWishlistDTO dto)
    {
        var item = await _wishlistService.AddToWishlistAsync(dto);
        return Created(
            item,
            "Added to wishlist successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _wishlistService.RemoveFromWishlistAsync(id);
        return NoContent();
    }
}
