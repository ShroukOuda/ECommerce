using ECommerce.Application.DTO.Shipping;

namespace ECommerce.Application.Interfaces;

public interface IShippingService
{
    Task<IEnumerable<GetShippingDTO>> GetShippingsByOrderIdAsync(int orderId, CancellationToken ct = default);
    Task<GetShippingDTO> GetShippingByIdAsync(int id, CancellationToken ct = default);
    Task<GetShippingDTO> CreateShippingAsync(CreateShippingDTO dto, CancellationToken ct = default);
}
