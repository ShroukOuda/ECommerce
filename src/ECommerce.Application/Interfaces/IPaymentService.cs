using ECommerce.Application.DTO.Payment;

namespace ECommerce.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(int orderId, CancellationToken ct = default);
    Task<GetPaymentDTO> GetPaymentByIdAsync(int id, CancellationToken ct = default);
    Task<GetPaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto, CancellationToken ct = default);
}
