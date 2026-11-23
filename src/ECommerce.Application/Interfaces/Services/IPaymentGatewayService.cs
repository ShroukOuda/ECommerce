using ECommerce.Application.DTO.Payment;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Interfaces.Services;

public interface IPaymentGatewayService
{
    Task<CreateCheckoutSessionResponseDTO> CreateCheckoutSessionAsync(
        Payment payment,
        Order order,
        CancellationToken cancellationToken = default);

    StripeWebhookEventDTO ParseWebhookEvent(string payload, string signatureHeader);
}