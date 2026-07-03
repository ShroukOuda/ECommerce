using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductVariantSpecification : BaseSpecification<ProductVariant, Guid>
{
    public ProductVariantSpecification(Guid productId)
        : base(pv => pv.ProductId == productId)
    {
        AddInclude(pv => pv.ProductImages);
        AddInclude(pv => pv.ProductVariantOptionValues);
        AsNoTracking();
    }

    
}