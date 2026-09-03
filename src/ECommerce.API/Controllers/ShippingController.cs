namespace ECommerce.API.Controllers;

[Route("api/v1/shipping")]
[Authorize(Roles = "Customer")]
public class ShippingController : BaseController
{
    private readonly IShippingService _shippingService;

    public ShippingController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;


    [HttpGet]
    public async Task<IActionResult> GetMyShippings()
    {
        var shippings =
            await _shippingService.GetMyShippingsAsync(
                CurrentUserId);

        return Success(
            shippings,
            "Shippings retrieved successfully.");
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var shipping =
            await _shippingService.GetMyShippingByIdAsync(
                id,
                CurrentUserId);

        return Success(
            shipping,
            "Shipping retrieved successfully.");
    }


    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(
        Guid orderId)
    {
        var shipping =
            await _shippingService.GetMyShippingByOrderIdAsync(
                orderId,
                CurrentUserId);

        return Success(
            shipping,
            "Shipping retrieved successfully.");
    }
}