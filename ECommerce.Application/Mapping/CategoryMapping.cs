using AutoMapper;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;

namespace E_Commerece.Application.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<AddCategoryDTO, Category>()
            .ReverseMap();
        CreateMap<UpdateCategoryDTO, Category>()
            .ReverseMap();
    }
}