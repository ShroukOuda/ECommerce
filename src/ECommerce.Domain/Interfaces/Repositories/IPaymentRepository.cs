using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, Guid>
{
    Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
