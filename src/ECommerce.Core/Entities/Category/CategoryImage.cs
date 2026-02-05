namespace ECommerce.Core.Entities.Category;

public class CategoryImage : BaseImage
{
    //FK
    public int CategoryId { get; set; }   
    public ImageSubType? SubType { get; set; }
    
    //Navigation Properties
    public virtual Category? Category { get; set; }
}