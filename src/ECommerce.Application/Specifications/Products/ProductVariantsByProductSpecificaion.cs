using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductVariantsByProductSpecification : BaseSpecification<ProductVariant, Guid>
{
    public ProductVariantsByProductSpecification(Guid productId)
        : base(pv => pv.ProductId == productId)
    {
        AddInclude(pv => pv.ProductImages);
        AddInclude(pv => pv.ProductVariantOptionValues);
        AsNoTracking();
    }

    
}