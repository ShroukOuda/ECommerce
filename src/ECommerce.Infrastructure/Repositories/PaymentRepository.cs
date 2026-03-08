using ECommerce.Core.Entities.Payment;

namespace ECommerce.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(int orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(ct);
    }
}
