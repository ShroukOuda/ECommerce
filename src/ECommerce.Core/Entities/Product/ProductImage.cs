namespace ECommerce.Core.Entities.Product;

public class ProductImage : BaseImage
{
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
   
    //FK
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    
    // Navigation Properties
    public virtual Product? Product { get; set; }
    public virtual ProductVariant? ProductVariant { get; set; }
}