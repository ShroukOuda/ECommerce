using ECommerce.Application.DTO.Order;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private string currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet()]
    public async Task<IActionResult> GetMyOrders()
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(currentUserId);
        return Success(
            orders,
            "Orders retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return Success(
            order,
            "Order retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> Create(CreateOrderDTO dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return Created(
            order,
            "Order created successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusDTO dto)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, dto);
        return Success(
            order,
            "Order status updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}
