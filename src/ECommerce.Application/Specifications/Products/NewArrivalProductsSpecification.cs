using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class NewArrivalProductsSpecification : BaseSpecification<Product, Guid>
{
    public NewArrivalProductsSpecification() 
        : base(p => p.IsNewArrival == true)
    {
        AsNoTracking();
        AddOrderByDescending(p => p.CreatedAt);
    }
    
}