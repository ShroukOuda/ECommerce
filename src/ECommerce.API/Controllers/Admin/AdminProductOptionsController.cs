using ECommerce.Application.DTO.ProductOption;


namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/product-options")]
[Authorize(Roles = "Admin")]
public class AdminProductOptionsController : BaseController
{
    private readonly IProductOptionService _productOptionService;

    public AdminProductOptionsController(IProductOptionService productOptionService)
    {
        _productOptionService = productOptionService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddProductOptionDTO dto)
    {
        var option =
            await _productOptionService.AddOptionAsync(dto);

        return Created(
            option,
            "Product option added successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductOptionDTO dto)
    {
        var option =
            await _productOptionService.UpdateOptionAsync(id, dto);

        return Success(
            option,
            "Product option updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productOptionService.DeleteOptionAsync(id);

        return NoContent();
    }

    [HttpPost("{optionId:guid}/values")]
    public async Task<IActionResult> AddValue(
        Guid optionId,
        AddProductOptionValueDTO dto)
    {
        var value =
            await _productOptionService.AddOptionValueAsync(
                optionId,
                dto);

        return Created(
            value,
            "Option value added successfully.");
    }

    [HttpDelete("{optionId:guid}/values/{valueId:guid}")]
    public async Task<IActionResult> DeleteValue(
        Guid optionId,
        Guid valueId)
    {
        await _productOptionService.DeleteOptionValueAsync(
            optionId,
            valueId);

        return NoContent();
    }
}