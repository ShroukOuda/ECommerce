using ECommerce.Application.DTO.Shipping;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class ShippingController : BaseController
{
    private readonly IShippingService _shippingService;

    public ShippingController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var shippings = await _shippingService.GetShippingsByOrderIdAsync(orderId);
        return Success(
            shippings,
            "Shippings retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var shipping = await _shippingService.GetShippingByIdAsync(id);
        return Success(
            shipping,
            "Shipping retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> Create(CreateShippingDTO dto)
    {
        var shipping = await _shippingService.CreateShippingAsync(dto);
        return Created(
            shipping,
            "Shipping created successfully.");
    }
}
