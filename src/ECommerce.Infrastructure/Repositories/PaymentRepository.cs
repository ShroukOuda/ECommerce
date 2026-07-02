using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment, Guid>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(ct);
    }
}
