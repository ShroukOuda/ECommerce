using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Orders;

public class OrderByUserSpecification : BaseSpecification<Order, Guid>
{
    public OrderByUserSpecification(Guid orderId, string userId)
        : base(o => o.UserId == userId && o.Id == orderId)
    {
        AddInclude(o => o.OrderItems);
        AddInclude(o => o.OrderStatusHistories);
        AddInclude(o => o.ShippingAddress);
        AddInclude(o => o.BillingAddress);
        AddInclude(o => o.Payments);
        AddInclude(o => o.Shippings);
        AsNoTracking();
    }
}