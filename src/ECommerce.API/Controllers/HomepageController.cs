using ECommerce.Application.DTO.Homepage;


namespace ECommerce.API.Controllers;

public class HomepageController : BaseController
{
    private readonly IHomePageService _homePageService;
    public HomepageController(IHomePageService homePageService)
    {
        _homePageService = homePageService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetHomePageData()
    {
        var homepageData = await _homePageService.GetHomePageDataAsync();
        return Success(
            homepageData,
            "Homepage data retrieved successfully.");
    }

}