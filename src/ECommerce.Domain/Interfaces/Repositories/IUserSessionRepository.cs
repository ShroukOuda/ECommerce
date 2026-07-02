using ECommerce.Domain.Entities.Users;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IUserSessionRepository : IGenericRepository<UserSession, Guid>
{
    Task<UserSession?> GetByRefreshTokenAsync(string refreshToken);
    Task<IReadOnlyList<UserSession>> GetActiveSessionsAsync(string userId);
    Task<IReadOnlyList<UserSession>> GetAllSessionsAsync(string userId);
}
