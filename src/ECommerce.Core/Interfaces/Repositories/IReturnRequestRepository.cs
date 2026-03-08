using ECommerce.Core.Entities.Return;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IReturnRequestRepository : IGenericRepository<ReturnRequest, int>
{
    Task<ReturnRequest?> GetReturnWithItemsAsync(int returnId, CancellationToken ct = default);
    Task<IReadOnlyList<ReturnRequest>> GetReturnsByUserIdAsync(string userId, CancellationToken ct = default);
}
