using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/returns")]
public class ReturnsController : BaseController
{
    private readonly IReturnService _returnService;

    public ReturnsController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    private string currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet()]
    public async Task<IActionResult> GetByUser()
    {
        var returns = await _returnService.GetReturnsByUserIdAsync(currentUserId);
        return Success(
            returns,
            "Returns retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ret = await _returnService.GetReturnByIdAsync(id);
        return Success(
            ret,
            "Return retrieved successfully.");
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateReturnRequestDTO dto)
    {
        var ret = await _returnService.CreateReturnRequestAsync(dto);
        return Created(
            ret,
            "Return request created successfully.");
    }
}
