namespace ECommerce.Core.Interfaces;

public interface IPhotoRepository : IGenericRepository<Photo, int>
{
    Task<Photo?> GetMainPhotoAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<Photo>> GetPhotosByEntityAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default);
    
    Task<Photo?> GetBySubTypeAsync(
        PhotoType type,
        string entityId,
        PhotoSubType subType,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<Photo>> GetPhotosByTypeAsync(
        PhotoType type,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<Photo>> GetPhotosBySubTypesAsync(
        PhotoType type,
        string entityId,
        IReadOnlyCollection<PhotoSubType> subTypes,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<Photo>> GetLatestPhotosAsync(
        int count,
        PhotoType? type = null,
        CancellationToken ct = default);

    Task<bool> EntityHasPhotosAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default);
    
    Task<bool> SubTypeExistsAsync(
        PhotoType type,
        string entityId,
        PhotoSubType subType,
        CancellationToken ct = default);
    
    Task<int> CountForEntityAsync(
        PhotoType type,
        string entityId,
        CancellationToken ct = default);
}