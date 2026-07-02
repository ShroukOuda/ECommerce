using ECommerce.Application.DTO.UserSession;
using ECommerce.Domain.Entities.Users;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserSessionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

   public async Task<IReadOnlyList<GetUserSessionDTO>> GetActiveSessionsAsync(string userId)
    {
        var sessions = await _unitOfWork.UserSessionRepository.GetActiveSessionsAsync(userId);
        var sessionDTOs = _mapper.Map<IReadOnlyList<GetUserSessionDTO>>(sessions); 
        return sessionDTOs;
    }
    public async Task<IReadOnlyList<GetUserSessionDTO>> GetAllSessionsAsync(string userId)
    {
        var sessions = await _unitOfWork.UserSessionRepository.GetAllSessionsAsync(userId);
        var sessionDTOs = _mapper.Map<IReadOnlyList<GetUserSessionDTO>>(sessions); 
        return sessionDTOs;
    }
 
    public async Task RevokeSessionAsync(Guid sessionId, string requestingUserId)
    {
        var session = await _unitOfWork.UserSessionRepository.GetByIdAsync(sessionId)
                      ?? throw new NotFoundException($"Session {sessionId} not found.");
 
        if (session.UserId != requestingUserId)
            throw new BadRequestException("You can only revoke your own sessions.");
 
        if (!session.IsActive)
            return; 
 
        session.IsActive  = false;
        session.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.UserSessionRepository.UpdateAsync(session);
        await _unitOfWork.SaveChangesAsync();
    }
 
    public async Task RevokeAllSessionsAsync(string userId)
    {
        var sessions = await _unitOfWork.UserSessionRepository.GetActiveSessionsAsync(userId);
        var now = DateTime.UtcNow;
 
        foreach (var session in sessions)
        {
            session.IsActive  = false;
            session.RevokedAt = now;
            await _unitOfWork.UserSessionRepository.UpdateAsync(session);
        }
 
        await _unitOfWork.SaveChangesAsync();
    }
 
   
}
