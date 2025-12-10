namespace ECommerce.Application.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<AddProductDTO, Product>()
            .ForMember(dest => dest.Photos, src=>src.Ignore())
            .ReverseMap();
        CreateMap<UpdateProductDTO, Product>().ReverseMap();
        CreateMap<Product, GetProductDTO>()
            .ForMember(dest=>dest.CategoryName, 
                src=>src.MapFrom(src=>src.Category.Name))
            .ReverseMap();
    }
}