using ECommerce.Domain.Entities.Category;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
        
    }
}