
using ECommerce.Domain.Enums.Category;

namespace ECommerce.Domain.Entities.Categories;

public class Category:BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CategoryStatus Status { get; set; } = CategoryStatus.Active;
    
    //FK
    public Guid? ParentCategoryId { get; set; } 

    // Navigation Properties
    public  Category ParentCategory { get; set; } = null!;
    public  ICollection<Category> ChildCategories { get; set; } = new List<Category>();
    public  ICollection<Product> Products { get; set; } = new List<Product>();
    public  ICollection<CategoryImage> CategoryImages { get; set; } = new List<CategoryImage>();
}