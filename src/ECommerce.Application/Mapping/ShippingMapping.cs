using ECommerce.Application.DTO.Shipping;
using ECommerce.Domain.Entities.Shippings;


namespace ECommerce.Application.Mapping;

public class ShippingMapping : Profile
{
    public ShippingMapping()
    {
        CreateMap<Shipping, GetShippingDTO>()
            .ForMember(d => d.Method, o => o.MapFrom(s => s.Method.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateShippingDTO, Shipping>();
            
        CreateMap<UpdateShippingDTO, Shipping>();
    }
}
