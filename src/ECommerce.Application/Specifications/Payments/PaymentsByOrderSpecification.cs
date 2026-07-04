using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Specifications.Payments;

public class PaymentsByOrderSpecification : BaseSpecification<Payment, Guid>
{
    public PaymentsByOrderSpecification(Guid orderId)
        : base(p => p.OrderId == orderId)
    {
        AddOrderByDescending(p => p.PaidAt);
        AsNoTracking();
    }

    
}