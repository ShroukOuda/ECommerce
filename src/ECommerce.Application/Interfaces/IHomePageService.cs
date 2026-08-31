using ECommerce.Application.DTO.Homepage;

namespace ECommerce.Application.Interfaces;

public interface IHomePageService
{
    Task<HomepageResponseDTO> GetHomePageDataAsync();
}