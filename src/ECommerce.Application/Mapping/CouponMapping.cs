using ECommerce.Application.DTO.Coupon;
using ECommerce.Domain.Entities.Coupons;

namespace ECommerce.Application.Mapping;

public class CouponMapping : Profile
{
    public CouponMapping()
    {
        CreateMap<AddCouponDTO, Coupon>();
          
        CreateMap<UpdateCouponDTO, Coupon>();
            
        CreateMap<Coupon, GetCouponDTO>();

        CreateMap<Coupon, GetCouponDetailsDTO>();
    }
}
