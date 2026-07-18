using ECommerce.Domain.Entities.Products;

namespace ECommerce.Application.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<AddProductDTO, Product>()
            .ForMember(dest => dest.ProductImages, src=>src.Ignore())
            .ReverseMap();
        CreateMap<UpdateProductDTO, Product>().ReverseMap();

        CreateMap<Product, GetProductsDTO>()
            .ForMember(dest=>dest.CategoryName, 
                src=>src.MapFrom(src=>src.Category.Name))
            .ForMember(dest=>dest.BrandName, 
                src=>src.MapFrom(src=>src.Brand.Name))
            .ForMember(dest=>dest.ProductMainImageUrl, 
                src=>src.MapFrom(src=>src.ProductImages.FirstOrDefault(pi=>pi.IsMain).ImageUrl))
            .ReverseMap();

        CreateMap<Product, GetProductDetailsDTO>()
            .ForMember(dest=>dest.CategoryName, 
                src=>src.MapFrom(src=>src.Category.Name))
            .ForMember(dest=>dest.BrandName, 
                src=>src.MapFrom(src=>src.Brand.Name))
            .ForMember(dest=>dest.ImageUrls, 
                src=>src.MapFrom(src=>src.ProductImages.Select(pi=>pi.ImageUrl).ToList()))
            .ReverseMap();
    }
}