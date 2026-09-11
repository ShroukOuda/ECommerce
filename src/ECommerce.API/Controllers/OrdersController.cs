using ECommerce.Application.DTO.Order;

namespace ECommerce.API.Controllers;

[Route("api/v1/orders")]
[Authorize]
public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(
            ClaimTypes.NameIdentifier)!;

    // GET: api/v1/orders
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var orders =
            await _orderService.GetMyOrdersAsync(
                CurrentUserId);

        return Success(
            orders,
            "Orders retrieved successfully.");
    }

    // GET: api/v1/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order =
            await _orderService.GetMyOrderByIdAsync(
                id,
                CurrentUserId);

        return Success(
            order,
            "Order retrieved successfully.");
    }

    // POST: api/v1/orders
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderDTO dto)
    {
        var order =
            await _orderService.CreateOrderAsync(
                CurrentUserId,
                dto);

        return Created(
            order,
            "Order created successfully.");
    }

    // DELETE: api/v1/orders/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _orderService.CancelOrderAsync(
            id,
            CurrentUserId);

        return NoContent();
    }
}