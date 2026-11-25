namespace ECommerce.Application.DTO.Shipping;

public class GetShippingStatusHistoryDTO
{
    public Guid Id { get; set; }
    public string ShippingStatus { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? Reason { get; set; }
    // Id of the user who made the change; empty for system changes.
    public string? ChangedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
