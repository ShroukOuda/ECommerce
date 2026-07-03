using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductOptionSpecification : BaseSpecification<ProductOption, Guid>
{
    public ProductOptionSpecification(Guid productId)
        : base(po => po.ProductId == productId)
    {
        AddInclude(po => po.ProductOptionValues);
        AddOrderBy(po => po.SortOrder);
        AsNoTracking();
    }

    
}