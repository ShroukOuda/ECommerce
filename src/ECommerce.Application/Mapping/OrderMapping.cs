using ECommerce.Application.DTO.Order;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Mapping;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, GetOrderDTO>()
            .ForMember(d => d.ShippingStatus, o => o.MapFrom(s => s.Shippings
                .Where(x => x.Status != ECommerce.Domain.Enums.Shipping.ShippingStatus.Failed)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Status.ToString())
                .FirstOrDefault()))
            .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.Payments
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Status.ToString())
                .FirstOrDefault()))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems))
            .ForMember(d => d.StatusHistory, o => o.MapFrom(s => s.OrderStatusHistories));
        
        CreateMap<Order, GetAdminOrderDTO>()
            .ForMember(d => d.ShippingStatus, o => o.MapFrom(s => s.Shippings
                .Where(x => x.Status != ECommerce.Domain.Enums.Shipping.ShippingStatus.Failed)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Status.ToString())
                .FirstOrDefault()))
            .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.Payments
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Status.ToString())
                .FirstOrDefault()))
            .ForMember(d => d.OrderItems, o => o.MapFrom(s => s.OrderItems))
            .ForMember(d => d.StatusHistory, o => o.MapFrom(s => s.OrderStatusHistories));

        CreateMap<OrderItem, GetOrderItemDTO>()
            .ForMember(d => d.Options, o => o.MapFrom(s => s.OrderItemOptions));

        CreateMap<OrderItemOption, GetOrderItemOptionDTO>();

        CreateMap<OrderStatusHistory, GetOrderStatusHistoryDTO>()
            .ForMember(d => d.ChangedBy, o => o.MapFrom(s => s.CreatedBy));
    }
}
