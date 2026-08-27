using ECommerce.Application.DTO.Product;

namespace ECommerce.Application.DTO.Homepage;

public class HomepageBannerDTO
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? AltText { get; set; }
}

public class HomepageCollectionDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class HomepageResponseDTO
{
    public IReadOnlyList<HomepageBannerDTO> Banners { get; set; } = Array.Empty<HomepageBannerDTO>();
    public IReadOnlyList<HomepageCollectionDTO> FeaturedCollections { get; set; } = Array.Empty<HomepageCollectionDTO>();
    public IReadOnlyList<GetProductsDTO> FeaturedProducts { get; set; } = Array.Empty<GetProductsDTO>();
    public IReadOnlyList<GetProductsDTO> NewArrivals { get; set; } = Array.Empty<GetProductsDTO>();
    public IReadOnlyList<GetProductsDTO> BestSellers { get; set; } = Array.Empty<GetProductsDTO>();
    public IReadOnlyList<GetProductsDTO> HotDeals { get; set; } = Array.Empty<GetProductsDTO>();
}
