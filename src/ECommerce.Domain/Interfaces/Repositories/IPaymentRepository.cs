using ECommerce.Domain.Entities.Payment;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, Guid>
{
    Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
