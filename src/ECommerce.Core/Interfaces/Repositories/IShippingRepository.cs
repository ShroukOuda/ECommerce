using ECommerce.Core.Entities.Shipping;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IShippingRepository : IGenericRepository<Shipping, Guid>
{
    Task<IReadOnlyList<Shipping>> GetShippingsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
