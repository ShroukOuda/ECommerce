using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.ProductImages;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums.Media;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;

[Route("api/v1/products")]
public class ProductsController : BaseController
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly INotificationSubscriptionService _notificationSubscriptionService;
    
    public ProductsController(
        IProductService productService, 
        IProductImageService productImageService, 
        INotificationSubscriptionService notificationSubscriptionService)
    {
        _productService =  productService;
        _productImageService = productImageService;
        _notificationSubscriptionService = notificationSubscriptionService;
    }


    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;


    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ProductSpecParams productParams)
    {
        var products = await _productService.GetAllProductsAsync(productParams);

        return Success(
            products,
            "Products retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Success(
            product,
            "Product retrieved successfully.");

    }

    [HttpGet("{productId:guid}/similar")]
    public async Task<IActionResult> GetSimilarProducts(Guid productId, [FromQuery] PaginationParams paginationParams)
    {
        var similarProducts = await _productService.GetSimilarProductsAsync(productId, paginationParams);
        return Success(
            similarProducts,
            "Similar products retrieved successfully.");
    }
    
    [HttpGet("{productId:guid}/images")]
    public async Task<IActionResult> GetProductImages(Guid productId)
    {
        var images = await _productImageService.GetProductImagesAsync(productId);
        return Success(
            images,
            "Product images retrieved successfully.");
    }

    [HttpPost("{productId:guid}/subscribe-stock-alert")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> SubscribeToStockAlert(Guid productId)
    {
        var userId = CurrentUserId;
        await _notificationSubscriptionService.SubscribeToStockAlertAsync(productId, userId);
        return SuccessMessage("Subscribed to stock alert successfully." + 
        "You will receive a notification when the product is back in stock.");
    }

    [HttpDelete("{productId:guid}/unsubscribe-stock-alert")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UnsubscribeFromStockAlert(Guid productId)
    {
        var userId = CurrentUserId;
        await _notificationSubscriptionService.UnsubscribeFromStockAlertAsync(productId, userId);
        return SuccessMessage("Unsubscribed from stock alert successfully.");
    }



}