using ECommerce.Application.DTO.ProductImages;

namespace ECommerce.Application.Mapping;

public class ProductImageMapping : Profile
{
    public ProductImageMapping()
    {
        CreateMap<ProductImage, ProductImageDTO>().ReverseMap();
    }
}