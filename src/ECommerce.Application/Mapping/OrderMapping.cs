using ECommerce.Application.DTO.Order;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Mapping;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, GetOrderDTO>()
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems))
            .ForMember(d => d.StatusHistory, o => o.MapFrom(s => s.OrderStatusHistories));
        
        CreateMap<Order, GetAdminOrderDTO>()
            .ForMember(d => d.OrderItems, o => o.MapFrom(s => s.OrderItems))
            .ForMember(d => d.StatusHistory, o => o.MapFrom(s => s.OrderStatusHistories));

        CreateMap<OrderItem, GetOrderItemDTO>()
            .ForMember(d => d.Options, o => o.MapFrom(s => s.OrderItemOptions));

        CreateMap<OrderItemOption, GetOrderItemOptionDTO>();

        CreateMap<OrderStatusHistory, GetOrderStatusHistoryDTO>();
    }
}
