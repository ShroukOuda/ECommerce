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

    [HttpGet("get-by-user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return Ok(order);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateOrderDTO dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return Ok(order);
    }

    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateStatus(UpdateOrderStatusDTO dto)
    {
        await _orderService.UpdateOrderStatusAsync(dto);
        return Ok(new ResponseAPI(200, "Order status updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return Ok(new ResponseAPI(200, "Order deleted successfully"));
    }
}
