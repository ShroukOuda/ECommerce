using ECommerce.Application.DTO.Order;

namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/orders")]
[Authorize(Roles = "Admin")]
public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/v1/admin/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders =
            await _orderService.GetAllOrdersAsync();

        return Success(
            orders,
            "Orders retrieved successfully.");
    }

    // GET: api/v1/admin/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order =
            await _orderService.GetOrderByIdAsync(id);

        return Success(
            order,
            "Order retrieved successfully.");
    }

    // PUT: api/v1/admin/orders/{id}/status
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateOrderStatusDTO dto)
    {
        var order =
            await _orderService.UpdateOrderStatusAsync(
                id,
                dto);

        return Success(
            order,
            "Order status updated successfully.");
    }
}