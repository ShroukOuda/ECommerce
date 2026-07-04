using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Application.Specifications.Categories;

public class CategorySpecification : BaseSpecification<Category, Guid>
{
    public CategorySpecification(Guid categoryId)
        : base(c => c.Id == categoryId)
    {
        AsNoTracking();
    }
    
}