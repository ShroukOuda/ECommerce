namespace ECommerce.API.Controllers;

[Route("api/v1/categories")]
public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) 
    {
        _categoryService = categoryService;
    }
    
    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Success(
            categories,
            "Categories retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Success(
            category,
            "Category retrieved successfully.");
    }

}