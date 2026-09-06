using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Domain.Enums.Media;

namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly ICategoryImageService _categoryImageService;
    public AdminCategoriesController(ICategoryService categoryService, ICategoryImageService categoryImageService) 
    {
        _categoryService = categoryService;
        _categoryImageService = categoryImageService;
    }
    

    [HttpPost()]
    public async Task<IActionResult> Add(AddCategoryDTO categoryDto)
    {
        var category = await _categoryService.AddCategoryAsync(categoryDto);
        return Created(
            category,
            "Category added successfully.");
    }
    
    [HttpPost("{categoryId:guid}/images")]
    public async Task<IActionResult> UploadImage(Guid categoryId, [FromForm] UploadCategoryImageDTO dto)
    {
        var image = await _categoryImageService.UploadImageAsync(categoryId, dto);
        return Created(
            image,
            "Category image uploaded successfully.");
    }
    

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDTO categoryDto)
    {
        await  _categoryService.UpdateCategoryAsync(id, categoryDto);
        return Success(
            categoryDto,
            "Category updated successfully.");
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
    
    [HttpDelete("{categoryId:guid}/images/{photoId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid photoId, Guid categoryId)
    {
        await _categoryImageService.DeleteCategoryImageAsync(categoryId, photoId);
        return NoContent();
    }

    [HttpDelete("{categoryId:guid}/images")]
    public async Task<IActionResult> DeleteImages(Guid categoryId)
    {
        await _categoryImageService.DeleteAllCategoryImagesAsync(categoryId);
        return NoContent();
    }
}