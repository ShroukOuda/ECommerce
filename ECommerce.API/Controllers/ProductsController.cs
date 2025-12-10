namespace E_Commerece.Api.Controllers;


public class ProductsController : BaseController
{
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService)
    {
        _productService =  productService;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] ProductParams productParams)
    {
        var products = await _productService.GetAllProductsAsync(productParams);
        int totalCount = await _productService.GetTotalCountAsync();
        var pagination = new Pagination<GetProductDTO>(productParams.PageNumber, productParams.PageSize, totalCount, products);
        return Ok(pagination);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddProductDTO productDTO)
    {
        await _productService.AddProductAsync(productDTO);
        return Ok("Added Successfully");
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductDTO productDTO)
    {
        await _productService.UpdateProductAsync(productDTO);
        return Ok("Updated Successfully");
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        return Ok("Deleted Successfully");
    }
}