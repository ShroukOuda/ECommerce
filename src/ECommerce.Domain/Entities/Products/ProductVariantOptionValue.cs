

namespace ECommerce.Domain.Entities.Products;

public class ProductVariantOptionValue : BaseEntity<Guid>
{
    //FK
    public Guid ProductVariantId { get; set; }
    public Guid ProductOptionValueId { get; set; }
    
    //Navigation Properties
    public  ProductVariant ProductVariant { get; set; } = null!;
    public  ProductOptionValue ProductOptionValue { get; set; } = null!;
}