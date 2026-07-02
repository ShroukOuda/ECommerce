using ECommerce.Application.DTO.Brand;
using ECommerce.Domain.Entities.Brands;

namespace ECommerce.Application.Mapping;

public class BrandMapping : Profile
{
    public BrandMapping()
    {
        CreateMap<AddBrandDTO, Brand>().ReverseMap();
        CreateMap<UpdateBrandDTO, Brand>().ReverseMap();
        CreateMap<Brand, GetBrandDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ReverseMap();
    }
}
