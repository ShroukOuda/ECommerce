namespace ECommerce.Application.DTO.Order;

public class GetOrderDTO 
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    // Computed from the order's shipment (null until a shipment exists). Never stored on the order.
    public string? ShippingStatus { get; set; }
    // Computed from the order's latest payment (null until a payment attempt exists).
    public string? PaymentStatus { get; set; }
    public string Currency { get; set; } = string.Empty;
    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GetOrderItemDTO> Items { get; set; } = new();
    public List<GetOrderStatusHistoryDTO> StatusHistory { get; set; } = new();
  
}
