using ECommerce.Application.DTO.UserSession;
using ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Interfaces;

public interface IUserSessionService
{
    public Task<IReadOnlyList<GetUserSessionDTO>> GetActiveSessionsAsync(string userId);
    public Task<IReadOnlyList<GetUserSessionDTO>> GetAllSessionsAsync(string userId);
    public Task RevokeSessionAsync(Guid sessionId, string requestingUserId);
    public Task RevokeAllSessionsAsync(string userId);

}
