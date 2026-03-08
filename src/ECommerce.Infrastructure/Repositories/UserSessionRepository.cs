using ECommerce.Core.Entities.User;

namespace ECommerce.Infrastructure.Repositories;

public class UserSessionRepository : GenericRepository<UserSession, int>, IUserSessionRepository
{
    public UserSessionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<UserSession>> GetSessionsByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
}
