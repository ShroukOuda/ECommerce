using ECommerce.Core.Entities.Product;

namespace ECommerce.Infrastructure.Repositories;

public class ProductOptionValueRepository : GenericRepository<ProductOptionValue, Guid>, IProductOptionValueRepository
{
    public ProductOptionValueRepository(AppDbContext context) : base(context) { }
}
