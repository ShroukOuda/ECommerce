using ECommerce.Domain.Entities.User;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IAddressRepository : IGenericRepository<Address, Guid>
{
    Task<IReadOnlyList<Address>> GetAddressesByUserIdAsync(string userId, CancellationToken ct = default);
}
