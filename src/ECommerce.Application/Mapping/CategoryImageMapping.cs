using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Core.Entities.Category;

namespace ECommerce.Application.Mapping;

public class CategoryImageMapping : Profile
{
    public CategoryImageMapping()
    {
        CreateMap<CategoryImage, CategoryImageDTO>().ReverseMap();
    }
}