namespace ECommerce.Core.Entities;

public class Category:BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}