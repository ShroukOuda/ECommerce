using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductOptionSpecification : BaseSpecification<ProductOption, Guid>
{
    public ProductOptionSpecification(Guid productOptionId)
        : base(po => po.Id == productOptionId)
    {
        AsNoTracking();
    }
    
}