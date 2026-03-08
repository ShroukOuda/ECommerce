using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class ProductVariantsController : BaseController
{
    private readonly IProductVariantService _productVariantService;

    public ProductVariantsController(IProductVariantService productVariantService)
    {
        _productVariantService = productVariantService;
    }

    [HttpGet("get-by-product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var variants = await _productVariantService.GetVariantsByProductIdAsync(productId);
        return Ok(variants);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var variant = await _productVariantService.GetVariantByIdAsync(id);
        return Ok(variant);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddProductVariantDTO dto)
    {
        await _productVariantService.AddVariantAsync(dto);
        return Ok(new ResponseAPI(200, "Product variant added successfully"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductVariantDTO dto)
    {
        await _productVariantService.UpdateVariantAsync(dto);
        return Ok(new ResponseAPI(200, "Product variant updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productVariantService.DeleteVariantAsync(id);
        return Ok(new ResponseAPI(200, "Product variant deleted successfully"));
    }
}
