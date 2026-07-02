using ECommerce.Domain.Entities.Returns;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IReturnRequestRepository : IGenericRepository<ReturnRequest, Guid>
{
    Task<ReturnRequest?> GetReturnWithItemsAsync(Guid returnId, CancellationToken ct = default);
    Task<IReadOnlyList<ReturnRequest>> GetReturnsByUserIdAsync(string userId, CancellationToken ct = default);
}
