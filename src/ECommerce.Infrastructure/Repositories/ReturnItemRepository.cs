using ECommerce.Core.Entities.Return;

namespace ECommerce.Infrastructure.Repositories;

public class ReturnItemRepository : GenericRepository<ReturnItem, int>, IReturnItemRepository
{
    public ReturnItemRepository(AppDbContext context) : base(context) { }
}
