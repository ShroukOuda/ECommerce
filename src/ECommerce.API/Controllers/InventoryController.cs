using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class InventoryController : BaseController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("history/{productId}")]
    public async Task<IActionResult> GetHistory(int productId)
    {
        var history = await _inventoryService.GetHistoryByProductIdAsync(productId);
        return Ok(history);
    }

    [HttpPost("add-history")]
    public async Task<IActionResult> AddHistory(CreateInventoryHistoryDTO dto)
    {
        await _inventoryService.AddInventoryHistoryAsync(dto);
        return Ok(new ResponseAPI(200, "Inventory history recorded successfully"));
    }
}
