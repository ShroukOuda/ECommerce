using ECommerce.Application.DTO.Return;


namespace ECommerce.API.Controllers;

[Authorize(Roles = "Customer")]
[Route("api/v1/returns")]
public class ReturnsController : BaseController
{
    private readonly IReturnService _returnService;

    public ReturnsController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;


    // GET: api/v1/returns
    [HttpGet]
    public async Task<IActionResult> GetMyReturns()
    {
        var returns =
            await _returnService.GetMyReturnsAsync(CurrentUserId);

        return Success(
            returns,
            "Returns retrieved successfully.");
    }


    // GET: api/v1/returns/{returnId}
    [HttpGet("{returnId:guid}")]
    public async Task<IActionResult> GetMyReturnById(
        Guid returnId)
    {
        var result =
            await _returnService.GetMyReturnByIdAsync(
                returnId,
                CurrentUserId);

        return Success(
            result,
            "Return retrieved successfully.");
    }


    // POST: api/v1/returns
    [HttpPost]
    public async Task<IActionResult> CreateReturn(
        [FromBody] CreateReturnRequestDTO dto)
    {
        var result =
            await _returnService.CreateReturnAsync(
                CurrentUserId,
                dto);

        return Created(
            result,
            "Return request created successfully.");
    }


    // PUT: api/v1/returns/{returnId}/cancel
    [HttpPut("{returnId:guid}/cancel")]
    public async Task<IActionResult> CancelReturn(
        Guid returnId)
    {
        var result =
            await _returnService.CancelReturnAsync(
                returnId,
                CurrentUserId);

        return Success(
            result,
            "Return request cancelled successfully.");
    }
}