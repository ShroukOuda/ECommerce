using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.DTO.Return;

public class ReturnHistoryResponseDTO
{
    public Guid Id { get; set; }
    public ReturnStatus? OldStatus { get; set; }
    public ReturnStatus NewStatus { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}