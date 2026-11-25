using ECommerce.Domain.Enums.Order;

namespace ECommerce.Application.DTO.Order;

public class GetOrderStatusHistoryDTO
{
    public Guid Id { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public string Source { get; set; } = string.Empty;

    public string? Reason { get; set; }

    // Id of the user who made the change; empty for system changes.
    public string? ChangedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}
