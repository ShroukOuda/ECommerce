using ECommerce.Core.Entities.Category;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
        
    }
}