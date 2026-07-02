using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category, Guid>
{
    
}