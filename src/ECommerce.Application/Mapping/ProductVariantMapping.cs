using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Product;

namespace ECommerce.Application.Mapping;

public class ProductVariantMapping : Profile
{
    public ProductVariantMapping()
    {
        CreateMap<AddProductVariantDTO, ProductVariant>();
        CreateMap<UpdateProductVariantDTO, ProductVariant>()
            .ForMember(d => d.Status, o => o.MapFrom(s => Enum.Parse<ProductVariantStatus>(s.Status)));
        CreateMap<ProductVariant, GetProductVariantDTO>()
            .ForMember(d => d.StockStatus, o => o.MapFrom(s => s.StockStatus.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
