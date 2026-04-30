using ECommerce.Domain.Entities.Order;

namespace ECommerce.Infrastructure.Repositories;

public class OrderItemOptionRepository : GenericRepository<OrderItemOption, Guid>, IOrderItemOptionRepository
{
    public OrderItemOptionRepository(AppDbContext context) : base(context) { }
}
