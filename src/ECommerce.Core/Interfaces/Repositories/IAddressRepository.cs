using ECommerce.Core.Entities.User;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IAddressRepository : IGenericRepository<Address, Guid>
{
    Task<IReadOnlyList<Address>> GetAddressesByUserIdAsync(string userId, CancellationToken ct = default);
}
