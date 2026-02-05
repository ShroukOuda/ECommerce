using ECommerce.Core.Enums.Category;

namespace ECommerce.Core.Entities.Category;

public class Category:BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CategoryStatus Status { get; set; } = CategoryStatus.Active;
    
    //FK
    public int? ParentCategoryId { get; set; } 

    // Navigation Properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product.Product> Products { get; set; } = new List<Product.Product>();
    public virtual ICollection<CategoryImage> CategoryImages { get; set; } = new List<CategoryImage>();
}