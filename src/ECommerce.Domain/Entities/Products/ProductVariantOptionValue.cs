using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Products;

public class ProductVariantOptionValue : BaseEntity<Guid>
{
    //FK
    public Guid ProductVariantId { get; set; }
    public Guid ProductOptionValueId { get; set; }
    
    //Navigation Properties
    public virtual ProductVariant? ProductVariant { get; set; }
    public virtual ProductOptionValue? ProductOptionValue { get; set; }
}