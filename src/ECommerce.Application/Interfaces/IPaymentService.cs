using ECommerce.Application.DTO.Payment;

namespace ECommerce.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId);
    Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id);
    Task<GetPaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto);
}
