using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Route("api/v1/product-options")]
public class ProductOptionsController : BaseController
{
    private readonly IProductOptionService _productOptionService;

    public ProductOptionsController(IProductOptionService productOptionService)
    {
        _productOptionService = productOptionService;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var options =
            await _productOptionService.GetOptionsByProductIdAsync(productId);

        return Success(
            options,
            "Product options retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var option =
            await _productOptionService.GetOptionByIdAsync(id);

        return Success(
            option,
            "Product option retrieved successfully.");
    }
}