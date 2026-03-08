using ECommerce.Application.DTO.Coupon;
using ECommerce.Core.Entities.Coupon;

namespace ECommerce.Application.Mapping;

public class CouponMapping : Profile
{
    public CouponMapping()
    {
        CreateMap<AddCouponDTO, Coupon>()
            .ForMember(d => d.DiscountType, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Coupon.DiscountType>(s.DiscountType)));
        CreateMap<UpdateCouponDTO, Coupon>()
            .ForMember(d => d.DiscountType, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Coupon.DiscountType>(s.DiscountType)));
        CreateMap<Coupon, GetCouponDTO>()
            .ForMember(d => d.DiscountType, o => o.MapFrom(s => s.DiscountType.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
