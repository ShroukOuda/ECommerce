using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class HotDealProductsSpecification : BaseSpecification<Product, Guid>
{
    public HotDealProductsSpecification() 
        : base(p => p.IsHotDeal == true)
    {
        AsNoTracking();
        AddOrderByDescending(p => p.CreatedAt);
    }
    
}