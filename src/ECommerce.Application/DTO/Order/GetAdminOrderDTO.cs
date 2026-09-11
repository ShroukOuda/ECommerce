using ECommerce.Domain.Enums.Order;

namespace ECommerce.Application.DTO.Order;

public class GetAdminOrderDTO
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public OrderType OrderType { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public OrderStatus OrderStatus { get; set; }

    public string Currency { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid? ShippingAddressId { get; set; }

    public Guid? BillingAddressId { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<GetOrderItemDTO> OrderItems { get; set; }
        = new List<GetOrderItemDTO>();

    public ICollection<GetOrderStatusHistoryDTO> StatusHistory { get; set; }
        = new List<GetOrderStatusHistoryDTO>();
}