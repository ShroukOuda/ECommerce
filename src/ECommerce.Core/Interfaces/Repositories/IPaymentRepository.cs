using ECommerce.Core.Entities.Payment;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, int>
{
    Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(int orderId, CancellationToken ct = default);
}
