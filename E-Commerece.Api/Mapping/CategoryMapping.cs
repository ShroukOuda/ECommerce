using AutoMapper;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;

namespace E_Commerece.Api.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<UpdateCategoryDTO, Category>().ReverseMap();
    }
}