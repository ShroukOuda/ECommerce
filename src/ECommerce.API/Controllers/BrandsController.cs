using ECommerce.Application.DTO.Brand;
using ECommerce.Application.DTO.BrandLogos;
using ECommerce.Domain.Enums.Media;

namespace ECommerce.API.Controllers;

public class BrandsController : BaseController
{
    private readonly IBrandService _brandService;
    private readonly IBrandLogoService _brandLogoService;

    public BrandsController(
        IBrandService brandService, 
        IBrandLogoService brandLogoService)
    {
        _brandService = brandService;
        _brandLogoService = brandLogoService;
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddBrandDTO dto)
    {
        await _brandService.AddBrandAsync(dto);
        return Ok(new ResponseAPI(200, "Brand added successfully"));
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(UpdateBrandDTO dto)
    {
        await _brandService.UpdateBrandAsync(dto);
        return Ok(new ResponseAPI(200, "Brand updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _brandService.DeleteBrandAsync(id);
        return Ok(new ResponseAPI(200, "Brand deleted successfully"));
    }

    [HttpPost("upload-logo")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadLogo(UploadBrandLogoDTO dto)
    {
        await _brandLogoService.UploadlogoAsync(dto);
        return Ok(new ResponseAPI(200, "Brand logo uploaded successfully"));
    }

    [HttpGet("get-logos/{brandId}")]
    public async Task<IActionResult> GetBrandLogos(Guid brandId)
    {
        var logos = await _brandLogoService.GetBrandLogosAsync(brandId);
        return Ok(logos);
    }

    [HttpGet("get-logo-by-sub-type")]
    public async Task<IActionResult> GetBrandLogoBySubType([FromQuery] Guid brandId, ImageSubType subType)
    {
        var logo = await _brandLogoService.GetBrandLogoBySubTypeAsync(brandId, subType);
        return Ok(logo);
    }

    [HttpGet("get-logo/{id}")]
    public async Task<IActionResult> GetBrandLogoById(Guid logoId)
    {
        var logo = await _brandLogoService.GetLogoByIdAsync(logoId);
        return Ok(logo);
    }

    [HttpDelete("delete-logo")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrandLogo(Guid brandId, Guid logoId)
    {
        await _brandLogoService.DeleteBrandLogoAsync(brandId, logoId);
        return Ok(new ResponseAPI(200, "Brand Logo deleted successfully"));
    }

    [HttpDelete("delete-logos")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrandLogos(Guid brandId)
    {
        await _brandLogoService.DeleteAllBrandLogosAsync(brandId);
        return Ok(new ResponseAPI(200, "Brand Logos deleted successfully"));
    }

}
