using ECommerce.Application.DTO.Shipping;
using ECommerce.Domain.Entities.Shippings;


namespace ECommerce.Application.Mapping;

public class ShippingMapping : Profile
{
    public ShippingMapping()
    {
        CreateMap<Shipping, GetShippingDTO>()
            .ForMember(d => d.Method, o => o.MapFrom(s => s.Method.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StatusHistory, o => o.MapFrom(s => s.StatusHistories.OrderBy(h => h.CreatedAt)));

        CreateMap<ShippingStatusHistory, GetShippingStatusHistoryDTO>()
            .ForMember(d => d.ShippingStatus, o => o.MapFrom(s => s.ShippingStatus.ToString()))
            .ForMember(d => d.ChangedBy, o => o.MapFrom(s => s.CreatedBy));

        CreateMap<CreateShippingDTO, Shipping>();
            
        CreateMap<UpdateShippingDTO, Shipping>();
    }
}
