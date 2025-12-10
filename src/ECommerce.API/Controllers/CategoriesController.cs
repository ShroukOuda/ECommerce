namespace ECommerce.API.Controllers;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService) 
    {
        _categoryService = categoryService;
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
}