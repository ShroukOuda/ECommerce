namespace ECommerce.Core.Entities;

public class CategoryImage : BaseImage
{
    //ForeignKey
    public int CategoryId { get; set; }   
    public ImageSubType? SubType { get; set; }
    
    // Navigation Properties
    public Category? Category { get; set; }
}