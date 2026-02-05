namespace ECommerce.Core.Entities.Product;

public class ProductVariantOptionValue
{
    //FK
    public int ProductVariantId { get; set; }
    public int ProductOptionValueId { get; set; }
    
    //Navigation Properties
    public virtual ProductVariant? ProductVariant { get; set; }
    public virtual ProductOptionValue? ProductOptionValue { get; set; }
}