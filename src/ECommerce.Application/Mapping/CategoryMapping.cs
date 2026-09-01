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

        CreateMap<Category, GetCategoryDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.CategoryImage, opt => opt.MapFrom(src => src.CategoryImages.Select(ci => ci.ImageUrl).ToList()))
            .ReverseMap();

        CreateMap<Category, GetCategoryDetailDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId))
            .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(src => src.ParentCategory))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.ChildCategories))
            .ForMember(dest => dest.CategoryImages, opt => opt.MapFrom(src => src.CategoryImages.Select(ci => ci.ImageUrl).ToList()))
            .ReverseMap();

    }
}