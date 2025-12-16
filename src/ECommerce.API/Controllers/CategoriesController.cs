using ECommerce.Core.Enums.Media;

namespace ECommerce.API.Controllers;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly IPhotoService _photoService;
    public CategoriesController(ICategoryService categoryService, IPhotoService photoService) 
    {
        _categoryService = categoryService;
        _photoService = photoService;
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
    public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoDTO uploadPhotoDto, CancellationToken ct = default)
    {
        await _photoService.UploadPhotoAsync(uploadPhotoDto, ct);
        return Ok(new ResponseAPI(200, "Category photo uploaded successfully"));
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
    public async Task<IActionResult> DeletePhoto(int photoId, CancellationToken ct = default)
    {
        await _photoService.DeletePhotoAsync(photoId, ct);
        return Ok(new ResponseAPI(200, "Category photo deleted successfully"));
    }

    [HttpDelete("delete-photos/{categoryId}")]
    public async Task<IActionResult> DeletePhotos(int categoryId, CancellationToken ct = default)
    {
        await _photoService.DeleteEntityPhotosAsync(PhotoType.CategoryMedia, categoryId, ct);
        return Ok(new ResponseAPI(200, "Category Media photos deleted successfully"));
    }
}