using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Notifications;


namespace ECommerce.Application.Specifications.Notifications;

public class ProductStockAlertSpecification : BaseSpecification<ProductStockAlert, Guid>
{
    public ProductStockAlertSpecification(Guid productId, string userId)
        : base(p => p.ProductId == productId && p.UserId == userId)
    {
        
    }
    
}