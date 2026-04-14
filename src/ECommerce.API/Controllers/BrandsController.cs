using ECommerce.Application.DTO.Brand;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class BrandsController : BaseController
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _brandService.GetAllBrandsAsync();
        return Ok(brands);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        return Ok(brand);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddBrandDTO dto)
    {
        await _brandService.AddBrandAsync(dto);
        return Ok(new ResponseAPI(200, "Brand added successfully"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateBrandDTO dto)
    {
        await _brandService.UpdateBrandAsync(dto);
        return Ok(new ResponseAPI(200, "Brand updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _brandService.DeleteBrandAsync(id);
        return Ok(new ResponseAPI(200, "Brand deleted successfully"));
    }
}
