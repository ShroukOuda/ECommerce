using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Category;

namespace ECommerce.Domain.Entities.Category;

public class Category:BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CategoryStatus Status { get; set; } = CategoryStatus.Active;
    
    //FK
    public Guid? ParentCategoryId { get; set; } 

    // Navigation Properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product.Product> Products { get; set; } = new List<Product.Product>();
    public virtual ICollection<CategoryImage> CategoryImages { get; set; } = new List<CategoryImage>();
}