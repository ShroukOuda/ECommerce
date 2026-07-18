namespace ECommerce.Domain.Entities.Categories;

public class CategoryImage : BaseImage
{
    //FK
    public Guid CategoryId { get; set; }   
    public ImageSubType SubType { get; set; }
    
    //Navigation Properties
    public  Category? Category { get; set; }
}