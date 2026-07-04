using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Inventories;

namespace ECommerce.Application.Specifications.Inventories;

public class InventoryHistoryByProductSpecification : BaseSpecification<InventoryHistory, Guid>
{
    public InventoryHistoryByProductSpecification(Guid productId)
        : base(ih => ih.ProductId == productId)
    {
        AddOrderByDescending(ih => ih.CreatedAt);
        AsNoTracking();
    }

    
}