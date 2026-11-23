using ECommerce.Application.DTO.BrandLogos;
using ECommerce.Domain.Entities.Brands;

namespace ECommerce.Application.Mapping;

public class BrandLogoMapping : Profile
{
    public BrandLogoMapping()
    {
        CreateMap<BrandLogo, BrandLogoDTO>()
            .ForMember(destination => destination.BrandName,
                options => options.MapFrom(source => source.Brand.Name))
            .ReverseMap()
            .ForMember(destination => destination.Brand,
                options => options.Ignore());
    }
}
