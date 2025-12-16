using ECommerce.Application.DTO.Photo;
using ECommerce.Core.Enums.Media;

namespace ECommerce.API.Controllers;


public class ProductsController : BaseController
{
    private readonly IProductService _productService;
    private readonly IPhotoService _photoService;
    
    public ProductsController(IProductService productService, IPhotoService photoService)
    {
        _productService =  productService;
        _photoService = photoService;
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
    public async Task<IActionResult> Add(AddProductDTO productDto)
    {
        await _productService.AddProductAsync(productDto);
        return Ok("Added Successfully");
    }
    
    [HttpPost("upload-photos")]
    public async Task<IActionResult> AddPhotos([FromForm] UploadPhotosDTO uploadPhotosDto, CancellationToken ct = default)
    {
        await _photoService.UploadPhotosAsync(uploadPhotosDto, ct);
        return Ok("Photos Uploaded Successfully");
    }

    [HttpPost("upload-photo")]
    public async Task<IActionResult> AddPhoto([FromForm] UploadPhotoDTO uploadPhotoDto, CancellationToken ct = default)
    {
        await _photoService.UploadPhotoAsync(uploadPhotoDto, ct);
        return Ok("Photo Uploaded Successfully");
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
    
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<IActionResult> DeletePhoto(int photoId, CancellationToken ct = default)
    {
        await _photoService.DeletePhotoAsync(photoId, ct);
        return Ok("Photo Deleted Successfully");
    }

    [HttpDelete("delete-photos/{productId}")]
    public async Task<IActionResult> DeletePhotos(int productId, CancellationToken ct = default)
    {
        await _photoService.DeleteEntityPhotosAsync(PhotoType.ProductImage, productId, ct);
        return Ok("Photos Deleted Successfully");
    }

}