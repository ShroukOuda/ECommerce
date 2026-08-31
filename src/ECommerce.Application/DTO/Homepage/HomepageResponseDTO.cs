
namespace ECommerce.Application.DTO.Homepage;
public class HomepageResponseDTO
{
    public IReadOnlyList<GetProductsDTO> FeaturedProducts { get; set; } = Array.Empty<GetProductsDTO>();
    public IReadOnlyList<GetProductsDTO> NewArrivals { get; set; } = Array.Empty<GetProductsDTO>();
    public IReadOnlyList<GetProductsDTO> BestSellers { get; set; } = Array.Empty<GetProductsDTO>();
    public IReadOnlyList<GetProductsDTO> HotDeals { get; set; } = Array.Empty<GetProductsDTO>();
}
