using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class CouponsController : BaseController
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return Ok(coupons);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var coupon = await _couponService.GetCouponByIdAsync(id);
        return Ok(coupon);
    }

    [HttpGet("get-by-code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var coupon = await _couponService.GetCouponByCodeAsync(code);
        return Ok(coupon);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddCouponDTO dto)
    {
        await _couponService.AddCouponAsync(dto);
        return Ok(new ResponseAPI(200, "Coupon added successfully"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCouponDTO dto)
    {
        await _couponService.UpdateCouponAsync(dto);
        return Ok(new ResponseAPI(200, "Coupon updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _couponService.DeleteCouponAsync(id);
        return Ok(new ResponseAPI(200, "Coupon deleted successfully"));
    }
}
