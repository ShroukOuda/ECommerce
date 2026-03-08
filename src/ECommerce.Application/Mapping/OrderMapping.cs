using ECommerce.Application.DTO.Order;
using ECommerce.Core.Entities.Order;

namespace ECommerce.Application.Mapping;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, GetOrderDTO>()
            .ForMember(d => d.OrderType, o => o.MapFrom(s => s.OrderType.ToString()))
            .ForMember(d => d.OrderStatus, o => o.MapFrom(s => s.OrderStatus.ToString()))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems));
        CreateMap<OrderItem, GetOrderItemDTO>();
        CreateMap<CreateOrderDTO, Order>();
    }
}
