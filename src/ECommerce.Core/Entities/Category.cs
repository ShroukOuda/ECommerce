namespace ECommerce.Core.Entities;

public class Category:BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<CategoryImage> CategoryImages { get; set; } = new List<CategoryImage>();
}