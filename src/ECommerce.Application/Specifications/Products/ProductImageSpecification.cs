using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductImageSpecification : BaseSpecification<ProductImage, Guid>
{
    public ProductImageSpecification(Guid productId)
        : base(i => i.ProductId == productId)
    {

       AddOrderBy(i => i.SortOrder);
       AsNoTracking();

    }

    public ProductImageSpecification(Guid productId, bool isMain)
        : base(i => i.ProductId == productId && i.IsMain == isMain)
    {
        AsNoTracking();
    }

    
}