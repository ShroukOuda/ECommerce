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
                opt => opt.MapFrom(src=>src.Category.Name))
            .ForMember(dest=>dest.BrandName, 
                opt => opt.MapFrom(src=>src.Brand.Name))
            .ForMember(dest => dest.ProductMainImageUrl, 
                opt => opt.MapFrom(src => src.ProductImages.First(i => i.IsMain).ImageUrl))
            .ForMember(dest=>dest.IsTopRated , 
                opt => opt.MapFrom(src => src.ReviewCount > 10 && src.AverageRating >= 4.5m))
            .ForMember(dest=>dest.IsBestSeller , 
                opt => opt.MapFrom(src=>src.TotalSales >= 100))
            .ReverseMap();

        CreateMap<Product, GetProductDetailsDTO>()
            .ForMember(dest=>dest.CategoryName, 
                opt => opt.MapFrom(src=>src.Category.Name))
            .ForMember(dest=>dest.BrandName, 
                opt => opt.MapFrom(src=>src.Brand.Name))
            .ForMember(dest=>dest.ImageUrls, 
                opt => opt.MapFrom(src=>src.ProductImages.Select(pi=>pi.ImageUrl).ToList()))
            .ForMember(dest=>dest.IsTopRated , 
                opt => opt.MapFrom(src => src.ReviewCount > 10 && src.AverageRating >= 4.5m))
            .ForMember(dest=>dest.IsBestSeller , 
                opt => opt.MapFrom(src=>src.TotalSales >= 100))
            .ReverseMap();
    }
}