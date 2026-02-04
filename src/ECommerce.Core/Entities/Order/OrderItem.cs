namespace ECommerce.Core.Entities.Order;

public class OrderItem : BaseEntity<int>
{
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public Dictionary<string, string>? VariantAttributes { get; set; } 
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    //FK
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    
    //Navigation Properties
    public virtual Order? Order { get; set; }
    public virtual Product.Product? Product { get; set; }
    public virtual Product.ProductVariant? ProductVariant { get; set; }
    public virtual ICollection<OrderItemOption> OrderItemOptions { get; set; } = new List<OrderItemOption>();
}