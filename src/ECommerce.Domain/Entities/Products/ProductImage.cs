

namespace ECommerce.Domain.Entities.Products;

public class ProductImage : BaseImage
{
    //FK
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    
    // Navigation Properties
    public virtual Product Product { get; set; } = null!;
    public virtual ProductVariant ProductVariant { get; set; } = null!;
}