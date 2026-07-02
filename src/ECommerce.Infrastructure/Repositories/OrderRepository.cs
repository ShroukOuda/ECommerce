using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order, Guid>, IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems).ThenInclude(oi => oi.OrderItemOptions)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
            .Include(o => o.OrderStatusHistories)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }
}
