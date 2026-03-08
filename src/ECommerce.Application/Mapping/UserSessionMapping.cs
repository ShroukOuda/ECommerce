using ECommerce.Application.DTO.UserSession;
using ECommerce.Core.Entities.User;

namespace ECommerce.Application.Mapping;

public class UserSessionMapping : Profile
{
    public UserSessionMapping()
    {
        CreateMap<UserSession, GetUserSessionDTO>();
    }
}
