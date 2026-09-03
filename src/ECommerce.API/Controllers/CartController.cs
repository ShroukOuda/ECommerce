using ECommerce.Application.DTO.Cart;
using ECommerce.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace ECommerce.API.Controllers;

[Route("api/v1/cart")]
public class CartController : BaseController
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet()]
    public async Task<IActionResult> GetMyCart()
    {
        var cart = await _cartService.GetActiveCartByUserIdAsync(CurrentUserId);
        return Success(
            cart,
            "Cart retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> AddItem(AddCartItemDTO dto)
    {
        var item = await _cartService.AddCartItemAsync(dto);
        return Created(item, "Item added to cart successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, UpdateCartItemDTO dto)
    {
        var item = await _cartService.UpdateCartItemAsync(id, dto);
        return Success(item, "Cart item updated successfully");
    }

    [HttpDelete("{cartId:guid}/items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartId, Guid cartItemId)
    {
        await _cartService.RemoveCartItemAsync(cartId, cartItemId);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> ClearCart(Guid id)
    {
        await _cartService.ClearCartAsync(id);
        return NoContent();
    }
}
