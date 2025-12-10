namespace ECommerce.Core.Entities;

public class Product:BaseEntity<int>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    
    //Navigation Properties
    public virtual Category Category { get; set; }
    public virtual List<Photo> Photos { get; set; }
}