using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Domain.Entities.Product;

namespace ECommerce.Application.Mapping;

public class ProductVariantMapping : Profile
{
    public ProductVariantMapping()
    {
        CreateMap<AddProductVariantDTO, ProductVariant>();
        CreateMap<UpdateProductVariantDTO, ProductVariant>()
            .ForMember(d => d.Status, o => o.MapFrom(s => Enum.Parse<ECommerce.Domain.Enums.Product.ProductVariantStatus>(s.Status)));
        CreateMap<ProductVariant, GetProductVariantDTO>()
            .ForMember(d => d.StockStatus, o => o.MapFrom(s => s.StockStatus.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
