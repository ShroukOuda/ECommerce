using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Returns;
using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.Specifications.Returns;

public class ReturnItemsByOrderSpecification : BaseSpecification<ReturnItem, Guid>
{
    public ReturnItemsByOrderSpecification(Guid orderId)
        : base(r => r.ReturnRequest.OrderId == orderId
            && r.Status != ReturnItemStatus.Rejected
            && r.ReturnRequest.Status != ReturnStatus.Rejected
            && r.ReturnRequest.Status != ReturnStatus.Cancelled)
    {
    }
}
