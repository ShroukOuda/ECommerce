using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
        
    }
}