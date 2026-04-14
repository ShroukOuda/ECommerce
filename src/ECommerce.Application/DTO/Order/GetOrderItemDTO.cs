namespace ECommerce.Application.DTO.Order;

public class GetOrderItemDTO
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
