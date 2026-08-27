using ECommerce.Application.DTO.Homepage;
using ECommerce.Application.DTO.Product;

namespace ECommerce.API.Controllers;

public class HomepageController : BaseController
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomepageController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHomepage()
    {
        var model = new HomepageResponseDTO
        {
            Banners = await GetBannersAsync(),
            FeaturedCollections = await GetFeaturedCollectionsAsync(),
            FeaturedProducts = await GetProductSetAsync(new ProductSpecParams { IsFeatured = true, PageNumber = 1, PageSize = 4 }),
            NewArrivals = await GetProductSetAsync(new ProductSpecParams { IsNewArrival = true, PageNumber = 1, PageSize = 4 }),
            BestSellers = await GetProductSetAsync(new ProductSpecParams { IsBestSeller = true, PageNumber = 1, PageSize = 4 }),
            HotDeals = await GetProductSetAsync(new ProductSpecParams { IsHotDeal = true, PageNumber = 1, PageSize = 4 })
        };

        return Ok(model);
    }

    [HttpGet("featured-collections")]
    public async Task<IActionResult> GetFeaturedCollections()
    {
        var collections = await GetFeaturedCollectionsAsync();
        return Ok(collections);
    }

    [HttpGet("banners")]
    public async Task<IActionResult> GetBanners()
    {
        return Ok(await GetBannersAsync());
    }

    [HttpGet("deals-today")]
    public async Task<IActionResult> GetDealsToday()
    {
        var deals = await GetProductSetAsync(new ProductSpecParams { IsHotDeal = true, PageNumber = 1, PageSize = 8 });
        return Ok(deals);
    }

    private async Task<IReadOnlyList<GetProductsDTO>> GetProductSetAsync(ProductSpecParams parameters)
    {
        try
        {
            var result = await _productService.GetAllProductsAsync(parameters);
            return result.Items ?? Array.Empty<GetProductsDTO>();
        }
        catch
        {
            return Array.Empty<GetProductsDTO>();
        }
    }

    private async Task<IReadOnlyList<HomepageCollectionDTO>> GetFeaturedCollectionsAsync()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return categories
                .Where(c => c.Status == ECommerce.Domain.Enums.Category.CategoryStatus.Active)
                .Take(6)
                .Select(c => new HomepageCollectionDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    ImageUrl = c.CategoryImages.FirstOrDefault()?.ImageUrl
                })
                .ToList();
        }
        catch
        {
            return Array.Empty<HomepageCollectionDTO>();
        }
    }

    private Task<IReadOnlyList<HomepageBannerDTO>> GetBannersAsync()
    {
        var banners = new List<HomepageBannerDTO>
        {
            new() { Title = "New Season Deals", ImageUrl = "/images/banners/seasonal-banner.jpg", Link = "/products?isFeatured=true", AltText = "Seasonal deals" },
            new() { Title = "Premium Tech", ImageUrl = "/images/banners/tech-banner.jpg", Link = "/products?isHotDeal=true", AltText = "Tech deals" },
            new() { Title = "Curated Collections", ImageUrl = "/images/banners/collection-banner.jpg", Link = "/collections/featured", AltText = "Featured collections" }
        };

        return Task.FromResult<IReadOnlyList<HomepageBannerDTO>>(banners);
    }
}
