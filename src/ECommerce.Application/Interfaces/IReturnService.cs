using ECommerce.Application.DTO.Return;

namespace ECommerce.Application.Interfaces;

public interface IReturnService
{
    // Customer
    Task<IReadOnlyList<ReturnResponseDTO>> GetMyReturnsAsync(string userId);

    Task<ReturnDetailsResponseDTO> GetMyReturnByIdAsync(
        Guid returnId,
        string userId);

    Task<ReturnDetailsResponseDTO> CreateReturnAsync(
        string userId,
        CreateReturnRequestDTO dto);

    Task<ReturnDetailsResponseDTO> CancelReturnAsync(
        Guid returnId,
        string userId);

    // Admin
    Task<IReadOnlyList<ReturnResponseDTO>> GetAllReturnsAsync();

    Task<AdminReturnDetailsResponseDTO> GetReturnByIdAsync(
        Guid returnId);

    Task<AdminReturnDetailsResponseDTO> ApproveReturnAsync(
        Guid returnId,
        string adminId);

    Task<AdminReturnDetailsResponseDTO> RejectReturnAsync(
        Guid returnId,
        string adminId,
        RejectReturnRequestDTO dto);

    Task<AdminReturnDetailsResponseDTO> MarkAsInTransitAsync(
        Guid returnId,
        string adminId);

    Task<AdminReturnDetailsResponseDTO> MarkAsReceivedAsync(
        Guid returnId,
        string adminId);

    Task<AdminReturnDetailsResponseDTO> StartProcessingAsync(
        Guid returnId,
        string adminId);

    Task<AdminReturnDetailsResponseDTO> ProcessRefundAsync(
        Guid returnId,
        string adminId,
        ProcessRefundRequestDTO dto);

    Task<AdminReturnDetailsResponseDTO> CompleteReturnAsync(
        Guid returnId,
        string adminId);

    // Return Items
    Task<ReturnItemResponseDTO> ApproveItemAsync(
        Guid returnItemId,
        string adminId);

    Task<ReturnItemResponseDTO> RejectItemAsync(
        Guid returnItemId,
        string adminId);

    Task<ReturnItemResponseDTO> MarkItemAsReceivedAsync(
        Guid returnItemId,
        string adminId);

    Task<ReturnItemResponseDTO> InspectItemAsync(
        Guid returnItemId,
        string adminId);

    Task<ReturnItemResponseDTO> RefundItemAsync(
        Guid returnItemId,
        string adminId);
}