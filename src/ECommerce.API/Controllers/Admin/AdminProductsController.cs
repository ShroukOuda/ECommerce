using ECommerce.Application.DTO.ProductImages;
using ECommerce.Application.Interfaces.Notifications;


namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : BaseController
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly INotificationSubscriptionService _notificationSubscriptionService;
    
    public AdminProductsController(
        IProductService productService, 
        IProductImageService productImageService, 
        INotificationSubscriptionService notificationSubscriptionService)
    {
        _productService =  productService;
        _productImageService = productImageService;
        _notificationSubscriptionService = notificationSubscriptionService;
    }

    
    [HttpPost]
    public async Task<IActionResult> Add(AddProductDTO productDto)
    {
        var product = await _productService.AddProductAsync(productDto);
        return Created(
            product,
            "Product added successfully.");
    }
    
    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddImage(Guid productId, [FromForm] UploadProductImageDTO dto)
    {
        var image = await _productImageService.UploadImageAsync(productId, dto);
        return Created(
            image,
            "Product image uploaded successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDTO productDTO)
    {
        var product = await _productService.UpdateProductAsync(id, productDTO);
        return Success(
            product,
            "Product updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
    
    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId)
    {
        await _productImageService.DeleteProductImageAsync(productId, imageId);
        return NoContent();
    }

    [HttpDelete("{productId:guid}/images")]
    public async Task<IActionResult> DeleteImages(Guid productId)
    {
        await _productImageService.DeleteAllProductImagesAsync(productId);
        return NoContent();
    }

}