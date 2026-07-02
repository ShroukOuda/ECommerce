using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Application.Mapping;

public class CategoryImageMapping : Profile
{
    public CategoryImageMapping()
    {
        CreateMap<CategoryImage, CategoryImageDTO>().ReverseMap();
    }
}