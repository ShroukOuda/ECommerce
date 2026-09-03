using ECommerce.Application.DTO.Homepage;


namespace ECommerce.API.Controllers;

[Route("api/v1/homepage")]
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