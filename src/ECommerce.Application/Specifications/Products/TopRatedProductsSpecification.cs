using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class TopRatedProductsSpecification : BaseSpecification<Product, Guid>
{
    public TopRatedProductsSpecification() 
        : base(p => p.IsTopRated == true)
    {
        AsNoTracking();
        AddOrderByDescending(p => p.CreatedAt);
    }
    
}