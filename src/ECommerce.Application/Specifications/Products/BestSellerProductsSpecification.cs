using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class BestSellerProductsSpecification : BaseSpecification<Product, Guid>
{
    public BestSellerProductsSpecification() 
        : base(p => p.IsBestSeller == true)
    {
        AsNoTracking();
        AddOrderByDescending(p => p.CreatedAt);
    }
    
}