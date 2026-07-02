using ECommerce.Domain.Entities.Users;


namespace ECommerce.Infrastructure.Repositories;

public class UserSessionRepository
    : GenericRepository<UserSession, Guid>, IUserSessionRepository
{
    public UserSessionRepository(AppDbContext context) : base(context) { }

    public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken)
        => await _dbSet
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

    public async Task<IReadOnlyList<UserSession>> GetActiveSessionsAsync(string userId)
        => await _dbSet
            .Where(s => s.UserId == userId &&
                        s.IsActive &&
                        s.RefreshTokenExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IReadOnlyList<UserSession>> GetAllSessionsAsync(string userId)
        => await _dbSet
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
}