using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductOptionValueSpecification : BaseSpecification<ProductOptionValue, Guid>
{
    public ProductOptionValueSpecification(Guid productOptionValueId)
        : base(pov => pov.Id == productOptionValueId)
    {
        AsNoTracking();
    }
    
}