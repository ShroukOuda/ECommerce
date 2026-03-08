using ECommerce.Core.Entities.Product;

namespace ECommerce.Infrastructure.Repositories;

public class ProductVariantOptionValueRepository : GenericRepository<ProductVariantOptionValue, int>, IProductVariantOptionValueRepository
{
    public ProductVariantOptionValueRepository(AppDbContext context) : base(context) { }
}
