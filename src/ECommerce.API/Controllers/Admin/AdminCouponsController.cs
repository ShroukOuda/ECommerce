using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

[Route("api/v1/admin/coupons")]
[Authorize(Roles = "Admin")]
public class AdminCouponsController : BaseController
{
    private readonly ICouponService _couponService;

    public AdminCouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return Success(
            coupons,
            "Coupons retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var coupon = await _couponService.GetCouponByIdAsync(id);
        return Success(
            coupon,
            "Coupon retrieved successfully.");
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var coupon = await _couponService.GetCouponByCodeAsync(code);
        return Success(
            coupon,
            "Coupon retrieved successfully.");
    }

    [HttpPost()]
    public async Task<IActionResult> Add(AddCouponDTO dto)
    {
        var coupon = await _couponService.AddCouponAsync(dto);
        return Created(
            coupon,
            "Coupon added successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponDTO dto)
    {
        var coupon = await _couponService.UpdateCouponAsync(id, dto);
        return Success(
            coupon,
            "Coupon updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _couponService.DeleteCouponAsync(id);
        return NoContent();
    }
}
