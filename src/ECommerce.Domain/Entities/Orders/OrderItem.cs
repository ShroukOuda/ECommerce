

namespace ECommerce.Domain.Entities.Orders;

public class OrderItem : BaseEntity<Guid>
{
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public Dictionary<string, string>? VariantAttributes { get; set; } 
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    //FK
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    
    //Navigation Properties
    public  Order Order { get; set; } = null!;
    public  Product Product { get; set; } = null!;
    public  ProductVariant ProductVariant { get; set; } = null!;
    public  ICollection<OrderItemOption> OrderItemOptions { get; set; } = new List<OrderItemOption>();
}