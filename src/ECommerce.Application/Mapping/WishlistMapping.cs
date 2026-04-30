using ECommerce.Application.DTO.Wishlist;
using ECommerce.Domain.Entities.Wishlist;

namespace ECommerce.Application.Mapping;

public class WishlistMapping : Profile
{
    public WishlistMapping()
    {
        CreateMap<AddWishlistDTO, Wishlist>();
        CreateMap<Wishlist, GetWishlistDTO>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
