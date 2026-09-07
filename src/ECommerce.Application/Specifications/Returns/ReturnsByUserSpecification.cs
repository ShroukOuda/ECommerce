using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Returns;

namespace ECommerce.Application.Specifications.Returns;

public class ReturnsByUserSpecification : BaseSpecification<ReturnRequest, Guid>
{
    public ReturnsByUserSpecification(string userId)
        : base(r => r.UserId == userId)
    {
        AsNoTracking();
    }

    
}