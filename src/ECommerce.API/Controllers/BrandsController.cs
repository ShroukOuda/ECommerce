namespace ECommerce.API.Controllers;

[Route("api/v1/brands")]
public class BrandsController : BaseController
{
    private readonly IBrandService _brandService;

    public BrandsController(
        IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _brandService.GetAllBrandsAsync();
        return Success(
            brands,
            "Brands retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        return Success(brand, "Brand retrieved successfully.");
    }

 

}
