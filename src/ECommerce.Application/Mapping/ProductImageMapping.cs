using ECommerce.Application.DTO.ProductImages;
using ECommerce.Domain.Entities.Product;

namespace ECommerce.Application.Mapping;

public class ProductImageMapping : Profile
{
    public ProductImageMapping()
    {
        CreateMap<ProductImage, ProductImageDTO>().ReverseMap();
    }
}