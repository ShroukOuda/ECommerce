using ECommerce.Core.Entities.Brand;

namespace ECommerce.Infrastructure.Repositories;

public class BrandRepository : GenericRepository<Brand, Guid>, IBrandRepository
{
    public BrandRepository(AppDbContext context) : base(context) { }
}
