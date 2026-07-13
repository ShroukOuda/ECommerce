using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Product;
using ECommerce.Domain.Enums.Inventory;


namespace ECommerce.Application.Specifications.Products;

public class LowStockProductsSpecification : BaseSpecification<Product, Guid>
{
    public LowStockProductsSpecification() 
        : base(p => p.StockStatus == StockStatus.LowStock)
    {
        AsNoTracking();
        AddOrderByDescending(p => p.CreatedAt);
    }
    
}