using System.Security.Claims;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/return-items")]
public class AdminReturnItemsController : BaseController
{
    private readonly IReturnService _returnService;

    public AdminReturnItemsController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    private string CurrentAdminId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;


    // PUT: api/v1/admin/return-items/{returnItemId}/approve
    [HttpPut("{returnItemId:guid}/approve")]
    public async Task<IActionResult> ApproveItem(
        Guid returnItemId)
    {
        var result =
            await _returnService.ApproveItemAsync(
                returnItemId,
                CurrentAdminId);

        return Success(
            result,
            "Return item approved successfully.");
    }


    // PUT: api/v1/admin/return-items/{returnItemId}/reject
    [HttpPut("{returnItemId:guid}/reject")]
    public async Task<IActionResult> RejectItem(
        Guid returnItemId)
    {
        var result =
            await _returnService.RejectItemAsync(
                returnItemId,
                CurrentAdminId);

        return Success(
            result,
            "Return item rejected successfully.");
    }


    // PUT: api/v1/admin/return-items/{returnItemId}/received
    [HttpPut("{returnItemId:guid}/received")]
    public async Task<IActionResult> MarkItemAsReceived(
        Guid returnItemId)
    {
        var result =
            await _returnService.MarkItemAsReceivedAsync(
                returnItemId,
                CurrentAdminId);

        return Success(
            result,
            "Return item marked as received successfully.");
    }


    // PUT: api/v1/admin/return-items/{returnItemId}/inspect
    [HttpPut("{returnItemId:guid}/inspect")]
    public async Task<IActionResult> InspectItem(
        Guid returnItemId)
    {
        var result =
            await _returnService.InspectItemAsync(
                returnItemId,
                CurrentAdminId);

        return Success(
            result,
            "Return item inspected successfully.");
    }


    // PUT: api/v1/admin/return-items/{returnItemId}/refund
    [HttpPut("{returnItemId:guid}/refund")]
    public async Task<IActionResult> RefundItem(
        Guid returnItemId)
    {
        var result =
            await _returnService.RefundItemAsync(
                returnItemId,
                CurrentAdminId);

        return Success(
            result,
            "Return item refunded successfully.");
    }
}