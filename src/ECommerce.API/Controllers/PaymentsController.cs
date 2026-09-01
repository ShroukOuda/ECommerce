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

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        return Success(
            payment,
            "Payment retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> Create(CreatePaymentDTO dto)
    {
        var payment = await _paymentService.CreatePaymentAsync(dto);
        return Created(
            payment,
            "Payment created successfully.");
    }
}
