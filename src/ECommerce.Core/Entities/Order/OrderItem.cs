namespace ECommerce.Core.Entities.Order;

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
    public virtual Order? Order { get; set; }
    public virtual Product.Product? Product { get; set; }
    public virtual Product.ProductVariant? ProductVariant { get; set; }
    public virtual ICollection<OrderItemOption> OrderItemOptions { get; set; } = new List<OrderItemOption>();
}