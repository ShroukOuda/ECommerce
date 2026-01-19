using ECommerce.Application.DTO.CategoryImages;

namespace ECommerce.Application.Mapping;

public class CategoryImageMapping : Profile
{
    public CategoryImageMapping()
    {
        CreateMap<CategoryImage, CategoryImageDTO>().ReverseMap();
    }
}