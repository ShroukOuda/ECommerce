using ECommerce.Application.DTO.UserSession;
using ECommerce.Domain.Entities.Users;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.UserSessions;

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
        var spec = new SessionsByUserSpecification(userId, true);
        var sessions = await _unitOfWork.GetRepository<UserSession, Guid>().GetAllAsync(spec);
        var sessionDTOs = _mapper.Map<IReadOnlyList<GetUserSessionDTO>>(sessions); 
        return sessionDTOs;
    }
    public async Task<IReadOnlyList<GetUserSessionDTO>> GetAllSessionsAsync(string userId)
    {
        var spec = new SessionsByUserSpecification(userId);
        var sessions = await _unitOfWork.GetRepository<UserSession, Guid>().GetAllAsync(spec);
        var sessionDTOs = _mapper.Map<IReadOnlyList<GetUserSessionDTO>>(sessions); 
        return sessionDTOs;
    }
 
    public async Task RevokeSessionAsync(Guid sessionId, string requestingUserId)
    {
        var session = await _unitOfWork.GetRepository<UserSession, Guid>().GetByIdAsync(sessionId)
                      ?? throw new NotFoundException($"Session {sessionId} not found.");
 
        if (session.UserId != requestingUserId)
            throw new BadRequestException("You can only revoke your own sessions.");
 
        if (!session.IsActive)
            return; 
 
        session.IsActive  = false;
        session.RevokedAt = DateTime.UtcNow;
        _unitOfWork.GetRepository<UserSession, Guid>().Update(session);
        await _unitOfWork.SaveChangesAsync();
    }
 
    public async Task RevokeAllSessionsAsync(string userId)
    {
        var spec = new SessionsByUserSpecification(userId, true);
        var sessions = await _unitOfWork.GetRepository<UserSession, Guid>().GetAllAsync(spec);
        var now = DateTime.UtcNow;
 
        foreach (var session in sessions)
        {
            session.IsActive  = false;
            session.RevokedAt = now;
            _unitOfWork.GetRepository<UserSession, Guid>().Update(session);
        }
 
        await _unitOfWork.SaveChangesAsync();
    }
 
   
}
