using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Domain.Enums.Media;

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
    
    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Success(
            categories,
            "Categories retrieved successfully.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Success(
            category,
            "Category retrieved successfully.");
    }

    [HttpPost()]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddCategoryDTO categoryDto)
    {
        var category = await _categoryService.AddCategoryAsync(categoryDto);
        return Created(
            category,
            "Category added successfully.");
    }
    
    [HttpPost("{categoryId:guid}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadImage(Guid categoryId, [FromForm] UploadCategoryImageDTO dto, CancellationToken ct = default)
    {
        var image = await _categoryImageService.UploadImageAsync(categoryId, dto, ct);
        return Created(
            image,
            "Category image uploaded successfully.");
    }

    [HttpGet("{categoryId:guid}/images")]
    public async Task<IActionResult> GetCategoryImages(Guid categoryId)
    {
        var Images = await _categoryImageService.GetCategoryImagesAsync(categoryId);
        return Success(
            Images,
            "Category images retrieved successfully.");
    }


    [HttpGet("{categoryId:guid}/images/sub-type")]
    public async Task<IActionResult> GetCategoryImageBySubType([FromQuery] Guid categoryId, ImageSubType subType)
    {
        var Image = await _categoryImageService.GetCategoryImageBySubTypeAsync(categoryId, subType);
        return Success(
            Image,
            "Category image retrieved successfully.");
    }

    [HttpGet("{categoryId:guid}/images/{id}")]
    public async Task<IActionResult> GetCategoryImageById(Guid categoryId, Guid ImageId)
    {
        var Image = await _categoryImageService.GetImageByIdAsync(categoryId, ImageId);
        return Success(
            Image,
            "Category image retrieved successfully.");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDTO categoryDto)
    {
        await  _categoryService.UpdateCategoryAsync(id, categoryDto);
        return Success(
            categoryDto,
            "Category updated successfully.");
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
    
    [HttpDelete("{categoryId:guid}/images/{photoId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(Guid photoId, Guid categoryId, CancellationToken ct = default)
    {
        await _categoryImageService.DeleteCategoryImageAsync(categoryId, photoId, ct);
        return NoContent();
    }

    [HttpDelete("{categoryId:guid}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImages(Guid categoryId, CancellationToken ct = default)
    {
        await _categoryImageService.DeleteAllCategoryImagesAsync(categoryId, ct);
        return NoContent();
    }
}