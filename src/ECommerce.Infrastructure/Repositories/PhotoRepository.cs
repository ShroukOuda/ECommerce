namespace ECommerce.Infrastructure.Repositories;

public class PhotoRepository : GenericRepository<Photo, int>, IPhotoRepository
{
   public PhotoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Photo?> GetMainPhotoAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Type == type &&
                     p.EntityId == entityId &&
                     p.IsMain,
                ct);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosByEntityAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Type == type && p.EntityId == entityId)
            .OrderByDescending(p => p.IsMain)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Photo?> GetBySubTypeAsync(
        PhotoType type,
        string entityId,
        PhotoSubType subType,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Type == type &&
                     p.EntityId == entityId &&
                     p.SubType == subType,
                ct);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosByTypeAsync(
        PhotoType type,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Type == type)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosBySubTypesAsync(
        PhotoType type,
        string entityId,
        IReadOnlyCollection<PhotoSubType> subTypes,
        CancellationToken ct = default)
    {
       
        var subTypeValues = subTypes.Cast<int>().ToArray();
        
        return await _dbSet
            .AsNoTracking()
            .Where(p =>
                p.Type == type &&
                p.EntityId == entityId &&
                subTypeValues.Contains((int)p.SubType))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Photo>> GetLatestPhotosAsync(
        int count,
        PhotoType? type = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking();

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<bool> EntityHasPhotosAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(
                p => p.Type == type && p.EntityId == entityId,
                ct);
    }

    public async Task<bool> SubTypeExistsAsync(
        PhotoType type,
        string entityId,
        PhotoSubType subType,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(
                p => p.Type == type &&
                     p.EntityId == entityId &&
                     p.SubType == subType,
                ct);
    }

    public async Task<int> CountForEntityAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(
                p => p.Type == type && p.EntityId == entityId,
                ct);
    }
}