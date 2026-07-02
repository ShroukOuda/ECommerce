using ECommerce.Domain.Entities.Returns;

namespace ECommerce.Infrastructure.Repositories;

public class ReturnItemRepository : GenericRepository<ReturnItem, Guid>, IReturnItemRepository
{
    public ReturnItemRepository(AppDbContext context) : base(context) { }
}
