using ECommerce.Application.DTO.UserSession;

namespace ECommerce.Application.Interfaces;

public interface IUserSessionService
{
    Task AddSessionAsync(AddUserSessionDTO sessionDto, CancellationToken ct = default);
    Task<IEnumerable<GetUserSessionDTO>> GetSessionsByUserIdAsync(string userId, CancellationToken ct = default);
    Task DeleteSessionAsync(Guid id, CancellationToken ct = default);
}
