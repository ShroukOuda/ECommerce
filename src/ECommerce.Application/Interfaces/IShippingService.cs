using ECommerce.Application.DTO.Shipping;

namespace ECommerce.Application.Interfaces;

public interface IShippingService
{
     // Customer
    Task<IEnumerable<GetShippingDTO>> GetMyShippingsAsync(
        string userId);

    Task<GetShippingDTO> GetMyShippingByIdAsync(
        Guid shippingId,
        string userId);

    Task<GetShippingDTO> GetMyShippingByOrderIdAsync(
        Guid orderId,
        string userId);


    // Admin
    Task<IEnumerable<GetShippingDTO>> GetAllShippingsAsync();

    Task<GetShippingDTO> GetShippingByIdAsync(
        Guid shippingId);

    Task<IEnumerable<GetShippingDTO>> GetShippingsByOrderIdAsync(
        Guid orderId);

    Task<GetShippingDTO> CreateShippingAsync(
        CreateShippingDTO dto);

    Task<GetShippingDTO> UpdateShippingAsync(
        Guid shippingId,
        UpdateShippingDTO dto);
}
