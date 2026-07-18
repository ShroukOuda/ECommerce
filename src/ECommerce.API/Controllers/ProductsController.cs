using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.ProductImages;
using ECommerce.Domain.Enums.Media;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;


public class ProductsController : BaseController
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    
    public ProductsController(IProductService productService, IProductImageService productImageService)
    {
        _productService =  productService;
        _productImageService = productImageService;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<PaginatedResult<GetProductsDTO>>> GetAll([FromQuery] ProductSpecParams productParams)
    {
        var products = await _productService.GetAllProductsAsync(productParams);
        return Ok(products);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    [HttpGet("get-similar/{productId}")]
    public async Task<IActionResult> GetSimilarProducts(Guid productId, [FromQuery] PaginationParams paginationParams)
    {
        var similarProducts = await _productService.GetSimilarProductsAsync(productId, paginationParams);
        return Ok(similarProducts);
    }
    
    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddProductDTO productDto)
    {
        await _productService.AddProductAsync(productDto);
        return Ok("Added Successfully");
    }
    
    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage([FromForm] UploadProductImageDTO dto, CancellationToken ct = default)
    {
        await _productImageService.UploadImageAsync(dto, ct);
        return Ok("Photo Uploaded Successfully");
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(UpdateProductDTO productDTO)
    {
        await _productService.UpdateProductAsync(productDTO);
        return Ok("Updated Successfully");
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return Ok("Deleted Successfully");
    }
    
    [HttpDelete("delete-photo/{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId, CancellationToken ct = default)
    {
        await _productImageService.DeleteProductImageAsync(productId, imageId);
        return Ok("Image Deleted Successfully");
    }

    [HttpDelete("delete-photos/{productId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImages(Guid productId, CancellationToken ct = default)
    {
        await _productImageService.DeleteAllProductImagesAsync(productId, ct);
        return Ok("Images Deleted Successfully");
    }

}