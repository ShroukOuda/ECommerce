using ECommerce.Domain.Entities.User;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IUserSessionRepository : IGenericRepository<UserSession, Guid>
{
    Task<IReadOnlyList<UserSession>> GetSessionsByUserIdAsync(string userId, CancellationToken ct = default);
}
