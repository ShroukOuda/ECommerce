namespace ECommerce.Application.Interfaces;

public interface IPhotoService
{
    Task<PhotoDTO> UploadPhotoAsync(
        UploadPhotoDTO uploadPhotoDto,
        CancellationToken ct);
    
    Task<IReadOnlyList<PhotoDTO>> UploadPhotosAsync(
        UploadPhotosDTO uploadPhotosDto,
        CancellationToken ct);
    
    Task DeletePhotoAsync(
        int photoId,
        CancellationToken ct);
    Task DeleteEntityPhotosAsync(
        PhotoType type, 
        int entityId,
        CancellationToken ct);
    
    Task SetMainPhotoAsync(
        int photoId,
        CancellationToken ct);
    
    Task<IReadOnlyList<PhotoDTO>> GetEntityPhotosAsync(
        PhotoType type, 
        int entityId,
        CancellationToken ct);
}