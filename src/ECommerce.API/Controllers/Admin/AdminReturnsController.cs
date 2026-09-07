using ECommerce.Application.DTO.Return;


namespace ECommerce.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/returns")]
public class AdminReturnsController : BaseController
{
    private readonly IReturnService _returnService;

    public AdminReturnsController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    private string CurrentAdminId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;


    // GET: api/v1/admin/returns
    [HttpGet]
    public async Task<IActionResult> GetAllReturns()
    {
        var returns =
            await _returnService.GetAllReturnsAsync();

        return Success(
            returns,
            "Returns retrieved successfully.");
    }


    // GET: api/v1/admin/returns/{returnId}
    [HttpGet("{returnId:guid}")]
    public async Task<IActionResult> GetReturnById(
        Guid returnId)
    {
        var result =
            await _returnService.GetReturnByIdAsync(
                returnId);

        return Success(
            result,
            "Return retrieved successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/approve
    [HttpPut("{returnId:guid}/approve")]
    public async Task<IActionResult> ApproveReturn(
        Guid returnId)
    {
        var result =
            await _returnService.ApproveReturnAsync(
                returnId,
                CurrentAdminId);

        return Success(
            result,
            "Return approved successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/reject
    [HttpPut("{returnId:guid}/reject")]
    public async Task<IActionResult> RejectReturn(
        Guid returnId,
        [FromBody] RejectReturnRequestDTO dto)
    {
        var result =
            await _returnService.RejectReturnAsync(
                returnId,
                CurrentAdminId,
                dto);

        return Success(
            result,
            "Return rejected successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/in-transit
    [HttpPut("{returnId:guid}/in-transit")]
    public async Task<IActionResult> MarkAsInTransit(
        Guid returnId)
    {
        var result =
            await _returnService.MarkAsInTransitAsync(
                returnId,
                CurrentAdminId);

        return Success(
            result,
            "Return marked as in transit successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/received
    [HttpPut("{returnId:guid}/received")]
    public async Task<IActionResult> MarkAsReceived(
        Guid returnId)
    {
        var result =
            await _returnService.MarkAsReceivedAsync(
                returnId,
                CurrentAdminId);

        return Success(
            result,
            "Return marked as received successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/processing
    [HttpPut("{returnId:guid}/processing")]
    public async Task<IActionResult> StartProcessing(
        Guid returnId)
    {
        var result =
            await _returnService.StartProcessingAsync(
                returnId,
                CurrentAdminId);

        return Success(
            result,
            "Return processing started successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/refund
    [HttpPut("{returnId:guid}/refund")]
    public async Task<IActionResult> ProcessRefund(
        Guid returnId,
        [FromBody] ProcessRefundRequestDTO dto)
    {
        var result =
            await _returnService.ProcessRefundAsync(
                returnId,
                CurrentAdminId,
                dto);

        return Success(
            result,
            "Refund processed successfully.");
    }


    // PUT: api/v1/admin/returns/{returnId}/complete
    [HttpPut("{returnId:guid}/complete")]
    public async Task<IActionResult> CompleteReturn(
        Guid returnId)
    {
        var result =
            await _returnService.CompleteReturnAsync(
                returnId,
                CurrentAdminId);

        return Success(
            result,
            "Return completed successfully.");
    }
}