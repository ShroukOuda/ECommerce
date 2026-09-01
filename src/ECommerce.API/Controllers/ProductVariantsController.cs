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

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var variants = await _productVariantService.GetVariantsByProductIdAsync(productId);
        return Success(
            variants,
            "Product variants retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var variant = await _productVariantService.GetVariantByIdAsync(id);
        return Success(
            variant,
            "Product variant retrieved successfully.");
    }

    [HttpPost()]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddProductVariantDTO dto)
    {
        var variant = await _productVariantService.AddVariantAsync(dto);
        return Created(
            variant,
            "Product variant added successfully.");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateProductVariantDTO dto)
    {
        var variant = await _productVariantService.UpdateVariantAsync(id, dto);
        return Success(
            variant,
            "Product variant updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productVariantService.DeleteVariantAsync(id);
        return NoContent();
    }
    
    [HttpGet("{id:guid}/sku/{sku}")]
    public async Task<IActionResult> GetBySKU(Guid id, string sku)
    {
        var variant = await _productVariantService.GetVariantBySKUAsync(id, sku);
        return Success(
            variant,
            "Product variant retrieved successfully.");
    }


}

   

