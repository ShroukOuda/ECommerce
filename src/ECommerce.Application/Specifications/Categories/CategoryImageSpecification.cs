using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Application.Specifications.Categories;

public class CategoryImageSpecification : BaseSpecification<CategoryImage, Guid>
{

    public CategoryImageSpecification(Guid categoryId, Guid imageId)
        : base(ci => ci.CategoryId == categoryId && ci.Id == imageId)
    {
        AsNoTracking();
    }
    public CategoryImageSpecification(Guid categoryId)
        : base(ci => ci.CategoryId == categoryId)
    {
        AddOrderBy(ci => ci.SubType);
        AsNoTracking();
    }

    public CategoryImageSpecification(Guid categoryId, ImageSubType subType)
        : base(ci => ci.CategoryId == categoryId && ci.SubType == subType)
    {
        AsNoTracking();
    }

    
}