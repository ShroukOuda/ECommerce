using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Core.Enums.Media;

namespace ECommerce.API.Controllers;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly ICategoryImageService _categoryImageService;
    public CategoriesController(ICategoryService categoryService, ICategoryImageService categoryImageService) 
    {
        _categoryService = categoryService;
        _categoryImageService = categoryImageService;
    }
    
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddCategoryDTO categoryDto)
    {
        await _categoryService.AddCategoryAsync(categoryDto);
        return Ok(new ResponseAPI(200, "Category added successfully"));
    }
    
    [HttpPost("uploadPhoto")]
    public async Task<IActionResult> UploadImage([FromForm] UploadCategoryImageDTO dto, CancellationToken ct = default)
    {
        await _categoryImageService.UploadImageAsync(dto, ct);
        return Ok(new ResponseAPI(200, "Category image uploaded successfully"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCategoryDTO categoryDto)
    {
        await  _categoryService.UpdateCategoryAsync(categoryDto);
        return Ok(new ResponseAPI(200, "Category updated successfully"));
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok(new ResponseAPI(200, "Category deleted successfully"));
    }
    
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<IActionResult> DeleteImage(int photoId, int categoryId, CancellationToken ct = default)
    {
        await _categoryImageService.DeleteCategoryImageAsync(categoryId, photoId, ct);
        return Ok(new ResponseAPI(200, "Category image deleted successfully"));
    }

    [HttpDelete("delete-photos/{categoryId}")]
    public async Task<IActionResult> DeleteImages(int categoryId, CancellationToken ct = default)
    {
        await _categoryImageService.DeleteAllCategoryImagesAsync(categoryId, ct);
        return Ok(new ResponseAPI(200, "Category Media images deleted successfully"));
    }
}