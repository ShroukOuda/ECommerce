using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Users;

namespace ECommerce.Application.Specifications.Users;

public class SessionsByUserSpecification : BaseSpecification<UserSession, Guid>
{
    public SessionsByUserSpecification(string userId)
        : base(s => s.UserId == userId)
    {
        AddOrderByDescending(s => s.CreatedAt);
        AsNoTracking();
    }

    public SessionsByUserSpecification(string userId, bool isActive)
        : base(s => s.UserId == userId 
        && s.IsActive == isActive 
        && s.RefreshTokenExpiresAt > DateTime.UtcNow)
    {
        AddOrderByDescending(s => s.CreatedAt);
        AsNoTracking();
    }

    
}
