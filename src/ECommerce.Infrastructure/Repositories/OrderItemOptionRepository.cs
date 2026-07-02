using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Infrastructure.Repositories;

public class OrderItemOptionRepository : GenericRepository<OrderItemOption, Guid>, IOrderItemOptionRepository
{
    public OrderItemOptionRepository(AppDbContext context) : base(context) { }
}
