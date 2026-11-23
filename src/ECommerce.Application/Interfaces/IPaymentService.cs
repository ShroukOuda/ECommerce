using ECommerce.Application.DTO.Payment;

namespace ECommerce.Application.Interfaces;

public interface IPaymentService
{
    Task<CreateCheckoutSessionResponseDTO> CreateCheckoutSessionAsync(Guid orderId, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId, string userId, bool isAdmin);
    Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id, string userId, bool isAdmin);
    Task HandleStripeWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default);
}
