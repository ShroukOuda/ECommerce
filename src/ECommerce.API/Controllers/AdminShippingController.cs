using ECommerce.Application.DTO.Shipping;


namespace ECommerce.API.Controllers;

[Route("api/v1/admin/shipping")]
[Authorize(Roles = "Admin")]
public class AdminShippingsController : BaseController
{
    private readonly IShippingService _shippingService;

    public AdminShippingsController(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var shippings =
            await _shippingService.GetAllShippingsAsync();

        return Success(
            shippings,
            "Shippings retrieved successfully.");
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var shipping =
            await _shippingService.GetShippingByIdAsync(
                id);

        return Success(
            shipping,
            "Shipping retrieved successfully.");
    }


    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(
        Guid orderId)
    {
        var shippings =
            await _shippingService.GetShippingsByOrderIdAsync(
                orderId);

        return Success(
            shippings,
            "Shippings retrieved successfully.");
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateShippingDTO dto)
    {
        var shipping =
            await _shippingService.CreateShippingAsync(
                dto);

        return Created(
            shipping,
            "Shipping created successfully.");
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateShippingDTO dto)
    {
        var shipping =
            await _shippingService.UpdateShippingAsync(
                id,
                dto);

        return Success(
            shipping,
            "Shipping updated successfully.");
    }
}