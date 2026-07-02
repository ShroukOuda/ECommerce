using ECommerce.Domain.Entities.Products;

namespace ECommerce.Infrastructure.Repositories;

public class ProductOptionValueRepository : GenericRepository<ProductOptionValue, Guid>, IProductOptionValueRepository
{
    public ProductOptionValueRepository(AppDbContext context) : base(context) { }
}
