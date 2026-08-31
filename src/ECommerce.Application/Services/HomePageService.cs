using ECommerce.Application.DTO.Homepage;

namespace ECommerce.Application.Services;

public class HomePageService : IHomePageService
{
    private readonly IProductService _productService;

    public HomePageService(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<HomepageResponseDTO> GetHomePageDataAsync()
    {
        var model = new HomepageResponseDTO
        {
            FeaturedProducts = await GetProductsAsync(new ProductSpecParams { IsFeatured = true, PageNumber = 1, PageSize = 4 }),
            NewArrivals = await GetProductsAsync(new ProductSpecParams { IsNewArrival = true, PageNumber = 1, PageSize = 4 }),
            BestSellers = await GetProductsAsync(new ProductSpecParams { IsBestSeller = true, PageNumber = 1, PageSize = 4 }),
            HotDeals = await GetProductsAsync(new ProductSpecParams { IsHotDeal = true, PageNumber = 1, PageSize = 4 })
        };

        return model;
    }

    private async Task<IReadOnlyList<GetProductsDTO>> GetProductsAsync( ProductSpecParams parameters) 
    { 
        var result = await _productService.GetAllProductsAsync(parameters); 
        return result.Items ?? Array.Empty<GetProductsDTO>();
    }




}