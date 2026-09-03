using ECommerce.Application.DTO.ProductOption;
using ECommerce.Application.Interfaces;

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
        var options = await _productOptionService.GetOptionsByProductIdAsync(productId);
        return Success(
            options,
            "Product options retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var option = await _productOptionService.GetOptionByIdAsync(id);
        return Success(
            option,
            "Product option retrieved successfully.");
    }


    [HttpPost()]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddProductOptionDTO dto)
    {
        var option = await _productOptionService.AddOptionAsync(dto);
        return Created(
            option,
            "Product option added successfully.");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateProductOptionDTO dto)
    {
        var option = await _productOptionService.UpdateOptionAsync(id, dto);
        return Success(
            option,
            "Product option updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productOptionService.DeleteOptionAsync(id);
        return NoContent();
    }

    [HttpPost("{optionId:guid}/values")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddValue(Guid optionId, AddProductOptionValueDTO dto)
    {
        var value = await _productOptionService.AddOptionValueAsync(optionId, dto);
        return Created(
            value,
            "Option value added successfully.");
    }

    [HttpDelete("{optionId:guid}/values/{valueId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteValue(Guid optionId, Guid valueId)
    {
        await _productOptionService.DeleteOptionValueAsync(optionId, valueId);
        return NoContent();
    }
}
