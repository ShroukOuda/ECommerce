using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class FeaturedProductsSpecification : BaseSpecification<Product, Guid>
{
    public FeaturedProductsSpecification() 
        : base(p => p.IsFeatured == true)
    {
        AsNoTracking();
        AddOrderByDescending(p => p.CreatedAt);
    }
    
}