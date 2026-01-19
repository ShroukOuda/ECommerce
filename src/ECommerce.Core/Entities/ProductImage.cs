namespace ECommerce.Core.Entities;

public class ProductImage : BaseImage
{
    //ForeignKey
    public int ProductId { get; set; }
    public bool IsMain { get; set; }
    
    // Navigation Properties
    public Product? Product { get; set; }
}