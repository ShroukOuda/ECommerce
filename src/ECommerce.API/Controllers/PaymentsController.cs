using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class PaymentsController : BaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("get-by-order/{orderId}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
        return Ok(payments);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        return Ok(payment);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreatePaymentDTO dto)
    {
        var payment = await _paymentService.CreatePaymentAsync(dto);
        return Ok(payment);
    }
}
