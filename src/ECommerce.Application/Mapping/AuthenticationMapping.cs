using ECommerce.Application.DTO.Auth;
using ECommerce.Domain.Entities.Users;

namespace Ecommerce.Application.Mapping;

public class AuthenticationMapping : Profile
{
    public AuthenticationMapping()
    {
        CreateMap<RegisterDTO, User>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ReverseMap();
    }
}