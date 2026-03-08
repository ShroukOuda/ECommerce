using ECommerce.Application.DTO.ProductOption;
using ECommerce.Core.Entities.Product;

namespace ECommerce.Application.Mapping;

public class ProductOptionMapping : Profile
{
    public ProductOptionMapping()
    {
        CreateMap<AddProductOptionDTO, ProductOption>()
            .ForMember(d => d.DisplayType, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Product.OptionDisplayType>(s.DisplayType)))
            .ForMember(d => d.Type, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Product.OptionType>(s.Type)));
        CreateMap<UpdateProductOptionDTO, ProductOption>()
            .ForMember(d => d.DisplayType, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Product.OptionDisplayType>(s.DisplayType)))
            .ForMember(d => d.Type, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Product.OptionType>(s.Type)));
        CreateMap<ProductOption, GetProductOptionDTO>()
            .ForMember(d => d.DisplayType, o => o.MapFrom(s => s.DisplayType.ToString()))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Values, o => o.MapFrom(s => s.ProductOptionValues));
        CreateMap<AddProductOptionValueDTO, ProductOptionValue>();
        CreateMap<ProductOptionValue, GetProductOptionValueDTO>();
    }
}
