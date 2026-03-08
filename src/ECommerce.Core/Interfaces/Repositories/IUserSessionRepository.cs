using ECommerce.Core.Entities.User;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IUserSessionRepository : IGenericRepository<UserSession, int>
{
    Task<IReadOnlyList<UserSession>> GetSessionsByUserIdAsync(string userId, CancellationToken ct = default);
}
