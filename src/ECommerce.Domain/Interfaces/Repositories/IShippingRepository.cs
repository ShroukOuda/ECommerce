using ECommerce.Domain.Entities.Shipping;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IShippingRepository : IGenericRepository<Shipping, Guid>
{
    Task<IReadOnlyList<Shipping>> GetShippingsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
