using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Product;

public class ProductImage : BaseImage
{
    //FK
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    
    // Navigation Properties
    public virtual Product? Product { get; set; }
    public virtual ProductVariant? ProductVariant { get; set; }
}