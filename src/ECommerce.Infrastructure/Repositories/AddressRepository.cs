using ECommerce.Domain.Entities.Users;

namespace ECommerce.Infrastructure.Repositories;

public class AddressRepository : GenericRepository<Address, Guid>, IAddressRepository
{
    public AddressRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Address>> GetAddressesByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(ct);
    }
}
