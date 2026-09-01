using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductVariantSpecification : BaseSpecification<ProductVariant, Guid>
{
    public ProductVariantSpecification(Guid productVariantId)
        : base(pv => pv.Id == productVariantId)
    {
        
    }

    public ProductVariantSpecification(Guid productId, string sku)
        : base(pv => pv.ProductId == productId && pv.Sku == sku)
    {
        
    }

    
}