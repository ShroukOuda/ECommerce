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

    [HttpGet("get-by-order/{orderId}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var shippings = await _shippingService.GetShippingsByOrderIdAsync(orderId);
        return Ok(shippings);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var shipping = await _shippingService.GetShippingByIdAsync(id);
        return Ok(shipping);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateShippingDTO dto)
    {
        var shipping = await _shippingService.CreateShippingAsync(dto);
        return Ok(shipping);
    }
}
