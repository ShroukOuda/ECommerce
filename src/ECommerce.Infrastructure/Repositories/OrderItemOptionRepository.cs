using ECommerce.Core.Entities.Order;

namespace ECommerce.Infrastructure.Repositories;

public class OrderItemOptionRepository : GenericRepository<OrderItemOption, int>, IOrderItemOptionRepository
{
    public OrderItemOptionRepository(AppDbContext context) : base(context) { }
}
