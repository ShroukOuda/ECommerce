using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Returns;

namespace ECommerce.Application.Specifications.Returns;

public class ReturnItemSpecification : BaseSpecification<ReturnItem, Guid>
{
    public ReturnItemSpecification(Guid returnItemId)
        : base(r => r.Id == returnItemId)
    {
        
    }

    
}