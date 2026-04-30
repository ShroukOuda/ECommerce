using ECommerce.Application.DTO.Address;
using ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Mapping;

public class AddressMapping : Profile
{
    public AddressMapping()
    {
        CreateMap<AddAddressDTO, Address>()
            .ForMember(d => d.Type, o => o.MapFrom(s => Enum.Parse<ECommerce.Domain.Enums.Address.AddressType>(s.Type)));
        CreateMap<UpdateAddressDTO, Address>()
            .ForMember(d => d.Type, o => o.MapFrom(s => Enum.Parse<ECommerce.Domain.Enums.Address.AddressType>(s.Type)));
        CreateMap<Address, GetAddressDTO>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
