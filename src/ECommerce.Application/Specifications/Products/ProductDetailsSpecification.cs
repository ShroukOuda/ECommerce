using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductDetailsSpecification : BaseSpecification<Product, Guid>
{
    public ProductDetailsSpecification(Guid productId)
        : base(p => p.Id == productId)
    {
        AddInclude(p => p.Category);
        AddInclude(p => p.Brand);
        AddInclude(p => p.ProductImages);

        AsNoTracking();
    }

    
}