using ECommerce.Application.DTO.Cart;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class CartController : BaseController
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var cart = await _cartService.GetActiveCartByUserIdAsync(userId);
        return Ok(cart);
    }

    [HttpPost("add-item")]
    public async Task<IActionResult> AddItem(AddCartItemDTO dto)
    {
        await _cartService.AddCartItemAsync(dto);
        return Ok(new ResponseAPI(200, "Item added to cart successfully"));
    }

    [HttpPut("update-item")]
    public async Task<IActionResult> UpdateItem(UpdateCartItemDTO dto)
    {
        await _cartService.UpdateCartItemAsync(dto);
        return Ok(new ResponseAPI(200, "Cart item updated successfully"));
    }

    [HttpDelete("remove-item/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        await _cartService.RemoveCartItemAsync(cartItemId);
        return Ok(new ResponseAPI(200, "Item removed from cart successfully"));
    }

    [HttpDelete("clear/{cartId}")]
    public async Task<IActionResult> ClearCart(int cartId)
    {
        await _cartService.ClearCartAsync(cartId);
        return Ok(new ResponseAPI(200, "Cart cleared successfully"));
    }
}
