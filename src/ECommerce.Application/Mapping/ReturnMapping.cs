using ECommerce.Application.DTO.Return;
using ECommerce.Domain.Entities.Return;

namespace ECommerce.Application.Mapping;

public class ReturnMapping : Profile
{
    public ReturnMapping()
    {
        CreateMap<ReturnRequest, GetReturnRequestDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.ReturnItems));
        CreateMap<ReturnItem, GetReturnItemDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
