using ECommerce.Application.DTO.ProductOption;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class ProductOptionsController : BaseController
{
    private readonly IProductOptionService _productOptionService;

    public ProductOptionsController(IProductOptionService productOptionService)
    {
        _productOptionService = productOptionService;
    }

    [HttpGet("get-by-product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var options = await _productOptionService.GetOptionsByProductIdAsync(productId);
        return Ok(options);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var option = await _productOptionService.GetOptionByIdAsync(id);
        return Ok(option);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddProductOptionDTO dto)
    {
        await _productOptionService.AddOptionAsync(dto);
        return Ok(new ResponseAPI(200, "Product option added successfully"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductOptionDTO dto)
    {
        await _productOptionService.UpdateOptionAsync(dto);
        return Ok(new ResponseAPI(200, "Product option updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productOptionService.DeleteOptionAsync(id);
        return Ok(new ResponseAPI(200, "Product option deleted successfully"));
    }

    [HttpPost("add-value")]
    public async Task<IActionResult> AddValue(AddProductOptionValueDTO dto)
    {
        await _productOptionService.AddOptionValueAsync(dto);
        return Ok(new ResponseAPI(200, "Option value added successfully"));
    }

    [HttpDelete("delete-value/{id}")]
    public async Task<IActionResult> DeleteValue(Guid id)
    {
        await _productOptionService.DeleteOptionValueAsync(id);
        return Ok(new ResponseAPI(200, "Option value deleted successfully"));
    }
}
