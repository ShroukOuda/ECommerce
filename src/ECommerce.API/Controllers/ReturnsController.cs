using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class ReturnsController : BaseController
{
    private readonly IReturnService _returnService;

    public ReturnsController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var returns = await _returnService.GetReturnsByUserIdAsync(userId);
        return Ok(returns);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ret = await _returnService.GetReturnByIdAsync(id);
        return Ok(ret);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateReturnRequestDTO dto)
    {
        var ret = await _returnService.CreateReturnRequestAsync(dto);
        return Ok(ret);
    }
}
