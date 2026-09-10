using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

[Route("api/v1/coupons")]
[Authorize(Roles = "Customer")]
public class CouponsController : BaseController
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(
        [FromBody] ValidateCouponDTO dto)
    {

        var result =
            await _couponService.ValidateCouponAsync(
                CurrentUserId,
                dto);

        return Success(
            result,
            "Coupon is valid.");
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply(
        [FromBody] ApplyCouponDTO dto)
    {
       
        var result =
            await _couponService.ApplyCouponAsync(
                CurrentUserId,
                dto);

        return Success(
            result,
            "Coupon applied successfully.");
    }
}