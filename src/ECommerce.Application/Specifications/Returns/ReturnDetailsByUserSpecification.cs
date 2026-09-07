using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Returns;

namespace ECommerce.Application.Specifications.Returns;

public class ReturnDetailsByUserSpecification : BaseSpecification<ReturnRequest, Guid>
{
    public ReturnDetailsByUserSpecification(Guid returnId, string userId)
        : base(r => r.Id == returnId && r.UserId == userId)
    {
        AddInclude(r => r.ReturnItems);
        AddInclude(r => r.ReturnHistories);
        AsNoTracking();
    }

    
}
     

