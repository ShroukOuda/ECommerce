using AutoMapper;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;

namespace E_Commerece.Api.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, GetProductDTO>()
            .ForMember(dest=>dest.CategoryName,
                src=>src.MapFrom(src=>src.Category.Name))
            .ReverseMap();
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<Product, UpdateProductDTO>().ReverseMap();
    }
}