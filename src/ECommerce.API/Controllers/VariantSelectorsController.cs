using ECommerce.Application.DTO.VariantSelectors;
using ECommerce.Application.Interfaces;
using ECommerce.Application.DTO.Product;

namespace ECommerce.API.Controllers;

public class VariantSelectorsController : BaseController
{
    private readonly IProductService _productService;
    private readonly IProductVariantService _variantService;

    public VariantSelectorsController(IProductService productService, IProductVariantService variantService)
    {
        _productService = productService;
        _variantService = variantService;
    }

    [HttpGet("{slug}/variant-selectors")]
    public async Task<IActionResult> GetVariantSelectors(string slug)
    {
        // find product by slug
        // product details DTO contains variant selectors indirectly via options/variants
        // For now return a lightweight response constructed from product and variants
        var prod = await _productService.GetAllProductsAsync(new ECommerce.Application.DTO.Product.ProductSpecParams { Search = slug, PageNumber = 1, PageSize = 1 });
        var item = prod.Items?.FirstOrDefault();
        if (item == null) return NotFoundResponse();

        // map basic structure; detailed selector building requires option/variant mapping
        var response = new VariantSelectorsResponseDTO
        {
            ProductId = item.Id,
            ProductName = item.Name,
            BasePrice = item.BasePrice,
            VariantSelectors = Array.Empty<VariantSelectorOptionDTO>()
        };

        return Ok(response);
    }

    [HttpPost("{slug}/find-variant")]
    public async Task<IActionResult> FindVariant(string slug, FindVariantRequestDTO request)
    {
        // Find product by slug
        var prodList = await _productService.GetAllProductsAsync(new ECommerce.Application.DTO.Product.ProductSpecParams { Search = slug, PageNumber = 1, PageSize = 1 });
        var product = prodList.Items?.FirstOrDefault();
        if (product is null) return NotFoundResponse();

        // Simplified: search for a variant that contains all option value ids (service currently supports by product id)
        var variants = await _variantService.GetVariantsByProductIdAsync(product.Id);
        var found = variants.FirstOrDefault(v => request.OptionValueIds.All(id => true)); // placeholder logic

        if (found is null)
        {
            return NotFound(new { success = false, error = new { code = "VARIANT_NOT_FOUND", message = "This combination is not available" }, suggestions = Array.Empty<object>() });
        }

        return Ok(new { success = true, data = new { variant = found } });
    }

    [HttpGet("{slug}/available-combinations")]
    public async Task<IActionResult> GetAvailableCombinations(string slug)
    {
        // Placeholder - return list of variants
        var prodList = await _productService.GetAllProductsAsync(new ECommerce.Application.DTO.Product.ProductSpecParams { Search = slug, PageNumber = 1, PageSize = 1 });
        var product = prodList.Items?.FirstOrDefault();
        if (product is null) return NotFoundResponse();

        var variants = await _variantService.GetVariantsByProductIdAsync(product.Id);
        return Ok(variants);
    }

    [HttpGet("{slug}/variants")]
    public async Task<IActionResult> GetVariants(string slug)
    {
        var prodList = await _productService.GetAllProductsAsync(new ECommerce.Application.DTO.Product.ProductSpecParams { Search = slug, PageNumber = 1, PageSize = 1 });
        var product = prodList.Items?.FirstOrDefault();
        if (product is null) return NotFoundResponse();

        var variants = await _variantService.GetVariantsByProductIdAsync(product.Id);
        return Ok(variants);
    }

    [HttpGet("{slug}/variants/{sku}")]
    public async Task<IActionResult> GetVariantBySku(string slug, string sku)
    {
        var prodList = await _productService.GetAllProductsAsync(new ECommerce.Application.DTO.Product.ProductSpecParams { Search = slug, PageNumber = 1, PageSize = 1 });
        var product = prodList.Items?.FirstOrDefault();
        if (product is null) return NotFoundResponse();

        var variants = await _variantService.GetVariantsByProductIdAsync(product.Id);
        var variant = variants.FirstOrDefault(v => string.Equals(v.Sku, sku, StringComparison.OrdinalIgnoreCase));
        if (variant is null) return NotFoundResponse();
        return Ok(variant);
    }
}
