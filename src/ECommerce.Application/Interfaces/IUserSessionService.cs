using ECommerce.Application.DTO.UserSession;

namespace ECommerce.Application.Interfaces;

public interface IUserSessionService
{
    Task<IEnumerable<GetUserSessionDTO>> GetSessionsByUserIdAsync(string userId, CancellationToken ct = default);
    Task DeleteSessionAsync(int id, CancellationToken ct = default);
}
