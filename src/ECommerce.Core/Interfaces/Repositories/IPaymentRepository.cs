using ECommerce.Core.Entities.Payment;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, Guid>
{
    Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
