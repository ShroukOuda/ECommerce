namespace ECommerce.Application.DTO.Return;
public class AdminReturnHistoryResponseDTO : ReturnHistoryResponseDTO
{
    public string PerformedById { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
}