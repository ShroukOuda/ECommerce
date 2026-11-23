using ECommerce.Application.DTO.Cart;
using ECommerce.Application.Interfaces;


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

    private string? GuestToken => Request.Headers["X-Cart-Token"].FirstOrDefault();

    [HttpPost("session")]
    public async Task<IActionResult> CreateCart() => Created(await _cartService.CreateCartAsync(CurrentUserId), "Cart created.");

    [HttpPost("{id:guid}/claim")]
    [Authorize]
    public async Task<IActionResult> Claim(Guid id) => Success(await _cartService.ClaimGuestCartAsync(id, GuestToken ?? "", CurrentUserId), "Cart claimed.");

    [HttpGet()]
    public async Task<IActionResult> GetMyCart([FromQuery] Guid? cartId = null)
    {
        var cart = cartId.HasValue ? await _cartService.GetCartByIdAsync(cartId.Value, CurrentUserId, GuestToken)
            : await _cartService.GetActiveCartByUserIdAsync(CurrentUserId);
        return Success(
            cart,
            "Cart retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> AddItem(AddCartItemDTO dto)
    {
        var item = await _cartService.AddCartItemAsync(dto, CurrentUserId, GuestToken);
        return Created(item, "Item added to cart successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, UpdateCartItemDTO dto)
    {
        var item = await _cartService.UpdateCartItemAsync(id, dto, CurrentUserId, GuestToken);
        return Success(item, "Cart item updated successfully");
    }

    [HttpDelete("{cartId:guid}/items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartId, Guid cartItemId)
    {
        await _cartService.RemoveCartItemAsync(cartId, cartItemId, CurrentUserId, GuestToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> ClearCart(Guid id)
    {
        await _cartService.ClearCartAsync(id, CurrentUserId, GuestToken);
        return NoContent();
    }
}
