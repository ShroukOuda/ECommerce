using ECommerce.Application.DTO.UserSession;
using ECommerce.Domain.Entities.Users;

namespace ECommerce.Application.Mapping;

public class UserSessionMapping : Profile
{
    public UserSessionMapping()
    {
        CreateMap<UserSession, GetUserSessionDTO>().ReverseMap();
      
    }
}
