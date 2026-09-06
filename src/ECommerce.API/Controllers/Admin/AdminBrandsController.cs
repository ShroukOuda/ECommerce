using ECommerce.Application.DTO.Brand;
using ECommerce.Application.DTO.BrandLogos;
using ECommerce.Domain.Enums.Media;

namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/brands")]
[Authorize(Roles = "Admin")]
public class AdminBrandsController : BaseController
{
    private readonly IBrandService _brandService;
    private readonly IBrandLogoService _brandLogoService;

    public AdminBrandsController(
        IBrandService brandService, 
        IBrandLogoService brandLogoService)
    {
        _brandService = brandService;
        _brandLogoService = brandLogoService;
    }


    [HttpPost()]
    public async Task<IActionResult> Add(AddBrandDTO dto)
    {
        var brand = await _brandService.AddBrandAsync(dto);
        return Created(brand, "Brand added successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBrandDTO dto)
    {
        var brand = await _brandService.UpdateBrandAsync(id, dto);
        return Success(brand, "Brand updated successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _brandService.DeleteBrandAsync(id);
        return NoContent();
    }

    [HttpPost("{brandId:guid}/logos")]
    public async Task<IActionResult> UploadLogo(Guid brandId, UploadBrandLogoDTO dto)
    {
        var logo = await _brandLogoService.UploadlogoAsync(brandId, dto);
        return Created(logo, "Brand logo uploaded successfully");
    }

    [HttpGet("{brandId:guid}/logos")]
    public async Task<IActionResult> GetBrandLogos(Guid brandId)
    {
        var logos = await _brandLogoService.GetBrandLogosAsync(brandId);
        return Success(
            logos,
            "Brand logos retrieved successfully.");
    }

    [HttpGet("{brandId:guid}/logos/{subType}")]
    public async Task<IActionResult> GetBrandLogoBySubType([FromQuery] Guid brandId, ImageSubType subType)
    {
        var logo = await _brandLogoService.GetBrandLogoBySubTypeAsync(brandId, subType);
        return Success(
            logo,
            "Brand logo retrieved successfully.");
    }

    [HttpGet("{brandId:guid}/logos/{id:guid}")]
    public async Task<IActionResult> GetBrandLogoById(Guid brandId, Guid logoId)
    {
        var logo = await _brandLogoService.GetLogoByIdAsync(brandId, logoId);
        return Success(
            logo,
            "Brand logo retrieved successfully.");
    }

    [HttpDelete("{brandId:guid}/logos/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrandLogo(Guid brandId, Guid logoId)
    {
        await _brandLogoService.DeleteBrandLogoAsync(brandId, logoId);
        return NoContent();
    }

    [HttpDelete("{brandId:guid}/logos")]
    public async Task<IActionResult> DeleteBrandLogos(Guid brandId)
    {
        await _brandLogoService.DeleteAllBrandLogosAsync(brandId);
        return NoContent();
    }

}
