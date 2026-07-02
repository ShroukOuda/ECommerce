using ECommerce.Domain.Entities.Products;

namespace ECommerce.Infrastructure.Repositories;

public class ProductVariantOptionValueRepository : GenericRepository<ProductVariantOptionValue, Guid>, IProductVariantOptionValueRepository
{
    public ProductVariantOptionValueRepository(AppDbContext context) : base(context) { }
}
