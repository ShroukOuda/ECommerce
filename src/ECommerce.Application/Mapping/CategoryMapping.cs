using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Application.Mapping;

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