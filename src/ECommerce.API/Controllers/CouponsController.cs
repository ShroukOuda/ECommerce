using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

[Route("api/v1/coupons")]
public class CouponsController : BaseController
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet()]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return Success(
            coupons,
            "Coupons retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var coupon = await _couponService.GetCouponByIdAsync(id);
        return Success(
            coupon,
            "Coupon retrieved successfully.");
    }

    [HttpGet("{code}")]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var coupon = await _couponService.GetCouponByCodeAsync(code);
        return Success(
            coupon,
            "Coupon retrieved successfully.");
    }

    [HttpPost()]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddCouponDTO dto)
    {
        var coupon = await _couponService.AddCouponAsync(dto);
        return Created(
            coupon,
            "Coupon added successfully.");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponDTO dto)
    {
        var coupon = await _couponService.UpdateCouponAsync(id, dto);
        return Success(
            coupon,
            "Coupon updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _couponService.DeleteCouponAsync(id);
        return NoContent();
    }
}
