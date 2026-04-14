using ECommerce.Application.DTO.Payment;

namespace ECommerce.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetPaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto, CancellationToken ct = default);
}
