using ECommerce.Application.DTO.UserSession;
using ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Mapping;

public class UserSessionMapping : Profile
{
    public UserSessionMapping()
    {
        CreateMap<UserSession, GetUserSessionDTO>().ReverseMap();
        CreateMap<AddUserSessionDTO, UserSession>().ReverseMap();
    }
}
