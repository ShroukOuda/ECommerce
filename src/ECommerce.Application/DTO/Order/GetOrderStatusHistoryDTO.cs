using ECommerce.Domain.Enums.Order;

namespace ECommerce.Application.DTO.Order;

public class GetOrderStatusHistoryDTO
{
    public Guid Id { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public DateTime CreatedAt { get; set; }
}