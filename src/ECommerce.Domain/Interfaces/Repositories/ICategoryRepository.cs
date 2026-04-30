using ECommerce.Domain.Entities.Category;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category, Guid>
{
    
}