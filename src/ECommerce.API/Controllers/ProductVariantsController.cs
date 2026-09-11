using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Route("api/v1/product-variants")]
public class ProductVariantsController : BaseController
{
    private readonly IProductVariantService _productVariantService;

    public ProductVariantsController(
        IProductVariantService productVariantService)
    {
        _productVariantService = productVariantService;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var variants =
            await _productVariantService
                .GetVariantsByProductIdAsync(productId);

        return Success(
            variants,
            "Product variants retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var variant =
            await _productVariantService
                .GetVariantByIdAsync(id);

        return Success(
            variant,
            "Product variant retrieved successfully.");
    }

    [HttpGet("product/{productId:guid}/sku/{sku}")]
    public async Task<IActionResult> GetBySku(Guid productId, string sku)
    {
        var variant =
            await _productVariantService.GetVariantBySKUAsync(productId, sku);

        return Success(
            variant,
            "Product variant retrieved successfully.");
    }
}