using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;

[Authorize]
[Route("api/v1/payments")]
public class PaymentsController : BaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private bool IsAdmin => User.IsInRole("Admin");

    [HttpPost("checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDTO dto, CancellationToken cancellationToken)
    {
        var checkoutSession = await _paymentService.CreateCheckoutSessionAsync(dto.OrderId, CurrentUserId, cancellationToken);
        return Created(checkoutSession, "Checkout session created successfully.");
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId, CurrentUserId, IsAdmin);
        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id, CurrentUserId, IsAdmin);
        return Success(
            payment,
            "Payment retrieved successfully.");
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    [Microsoft.AspNetCore.RateLimiting.DisableRateLimiting]
    public async Task<IActionResult> Webhook(CancellationToken cancellationToken)
    {
        Request.EnableBuffering();

        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;

        var signature = Request.Headers["Stripe-Signature"].ToString();

        await _paymentService.HandleStripeWebhookAsync(payload, signature, cancellationToken);

        return Ok(new { success = true });
    }
}
