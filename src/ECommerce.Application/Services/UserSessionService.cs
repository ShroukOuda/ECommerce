using ECommerce.Application.DTO.UserSession;
using ECommerce.Domain.Entities.User;
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

    public async Task AddSessionAsync(AddUserSessionDTO sessionDto, CancellationToken ct = default)
    {
        var session = _mapper.Map<UserSession>(sessionDto);
        session.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.UserSessionRepository.AddAsync(session, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task<IEnumerable<GetUserSessionDTO>> GetSessionsByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var sessions = await _unitOfWork.UserSessionRepository.GetSessionsByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<GetUserSessionDTO>>(sessions);
    }

    public async Task DeleteSessionAsync(Guid id, CancellationToken ct = default)
    {
        var session = await _unitOfWork.UserSessionRepository.GetByIdAsync(id, ct);
        if (session is null) throw new KeyNotFoundException($"Session with ID {id} not found.");
        await _unitOfWork.UserSessionRepository.DeleteAsync(session, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
