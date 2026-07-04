using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Users;

namespace ECommerce.Application.Specifications.Users;

public class SessionByRefreshTokenSpecification : BaseSpecification<UserSession, Guid>
{
    public SessionByRefreshTokenSpecification(string refreshToken)
        : base(s => s.RefreshToken == refreshToken)
    {
        AddInclude(s => s.User);
        AsNoTracking();
    }
}


