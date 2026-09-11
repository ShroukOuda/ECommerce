using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/product-variants")]
[Authorize(Roles = "Admin")]
public class ProductVariantsController : BaseController
{
    private readonly IProductVariantService _productVariantService;

    public ProductVariantsController(
        IProductVariantService productVariantService)
    {
        _productVariantService = productVariantService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        AddProductVariantDTO dto)
    {
        var variant =
            await _productVariantService
                .AddVariantAsync(dto);

        return Created(
            variant,
            "Product variant added successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductVariantDTO dto)
    {
        var variant =
            await _productVariantService
                .UpdateVariantAsync(id, dto);

        return Success(
            variant,
            "Product variant updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productVariantService
            .DeleteVariantAsync(id);

        return NoContent();
    }
}