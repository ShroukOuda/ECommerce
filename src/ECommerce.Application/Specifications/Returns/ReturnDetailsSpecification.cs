using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Returns;

namespace ECommerce.Application.Specifications.Returns;

public class ReturnDetailsSpecification : BaseSpecification<ReturnRequest, Guid>
{
    public ReturnDetailsSpecification(Guid returnId)
        : base(r => r.Id == returnId)
    {
        AddInclude(r => r.ReturnItems);
        AsNoTracking();
    }

    
}
     

