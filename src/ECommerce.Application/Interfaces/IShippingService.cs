using ECommerce.Application.DTO.Shipping;

namespace ECommerce.Application.Interfaces;

public interface IShippingService
{
    Task<IEnumerable<GetShippingDTO>> GetShippingsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<GetShippingDTO> GetShippingByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetShippingDTO> CreateShippingAsync(CreateShippingDTO dto, CancellationToken ct = default);
}
