using ECommerce.Core.Entities.Shipping;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IShippingRepository : IGenericRepository<Shipping, int>
{
    Task<IReadOnlyList<Shipping>> GetShippingsByOrderIdAsync(int orderId, CancellationToken ct = default);
}
