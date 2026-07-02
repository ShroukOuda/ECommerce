using ECommerce.Application.DTO.Cart;
using ECommerce.Domain.Entities.Carts;

namespace ECommerce.Application.Mapping;

public class CartMapping : Profile
{
    public CartMapping()
    {
        CreateMap<Cart, GetCartDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.CartItems));
        CreateMap<CartItem, GetCartItemDTO>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty));
        CreateMap<AddCartItemDTO, CartItem>();
    }
}
