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
    public async Task<IActionResult> GetAll([FromQuery] ProductParams productParams)
    {
        var products = await _productService.GetAllProductsAsync(productParams);
        var pagination = new Pagination<GetProductDTO>(productParams.PageNumber, productParams.PageSize, products.TotalCount, products.Products);
        return Ok(pagination);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(AddProductDTO productDto)
    {
        await _productService.AddProductAsync(productDto);
        return Ok("Added Successfully");
    }
    
    [HttpPost("upload-image")]
    public async Task<IActionResult> AddImage([FromForm] UploadProductImageDTO dto, CancellationToken ct = default)
    {
        await _productImageService.UploadImageAsync(dto, ct);
        return Ok("Photo Uploaded Successfully");
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductDTO productDTO)
    {
        await _productService.UpdateProductAsync(productDTO);
        return Ok("Updated Successfully");
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return Ok("Deleted Successfully");
    }
    
    [HttpDelete("delete-photo/{imageId}")]
    public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId, CancellationToken ct = default)
    {
        await _productImageService.DeleteProductImageAsync(productId, imageId);
        return Ok("Image Deleted Successfully");
    }

    [HttpDelete("delete-photos/{productId}")]
    public async Task<IActionResult> DeleteImages(Guid productId, CancellationToken ct = default)
    {
        await _productImageService.DeleteAllProductImagesAsync(productId, ct);
        return Ok("Images Deleted Successfully");
    }

}