using ECommerce.Application.DTO.Inventory;


namespace ECommerce.API.Controllers.Admin;

[Route("api/v1/admin/inventory")]
[Authorize(Roles = "Admin")]
public class InventoryController : BaseController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{productId:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid productId)
    {
        var history =
            await _inventoryService.GetHistoryByProductIdAsync(productId);

        return Success(
            history,
            "Inventory history retrieved successfully.");
    }

    [HttpPost("{productId:guid}/history")]
    public async Task<IActionResult> AddHistory(
        Guid productId,
        CreateInventoryHistoryDTO dto)
    {
        var history =
            await _inventoryService.AddInventoryHistoryAsync(
                productId,
                CurrentUserId,
                dto);

        return Created(
            history,
            "Inventory history added successfully.");
    }
}