using ECommerce.Domain.Entities.Return;

namespace ECommerce.Infrastructure.Repositories;

public class ReturnRequestRepository : GenericRepository<ReturnRequest, Guid>, IReturnRequestRepository
{
    private readonly AppDbContext _context;

    public ReturnRequestRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ReturnRequest?> GetReturnWithItemsAsync(Guid returnId, CancellationToken ct = default)
    {
        return await _context.ReturnRequests
            .Include(r => r.ReturnItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == returnId, ct);
    }

    public async Task<IReadOnlyList<ReturnRequest>> GetReturnsByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.ReturnRequests
            .Include(r => r.ReturnItems)
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RequestedDate)
            .ToListAsync(ct);
    }
}
