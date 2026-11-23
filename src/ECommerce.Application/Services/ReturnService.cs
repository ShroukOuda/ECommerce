using AutoMapper;
using FluentValidation;
using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Specifications.Coupons;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Application.Specifications.Returns;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Returns;
using ECommerce.Domain.Enums.Return;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class ReturnService : IReturnService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateReturnRequestDTO> _createValidator;

    public ReturnService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateReturnRequestDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    // =========================================================
    // CUSTOMER
    // =========================================================

    public async Task<IReadOnlyList<ReturnResponseDTO>>
        GetMyReturnsAsync(string userId)
    {
        var spec = new ReturnsByUserSpecification(userId);

        var returns = await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<ReturnResponseDTO>>(returns);
    }

    public async Task<ReturnDetailsResponseDTO>
        GetMyReturnByIdAsync(
            Guid returnId,
            string userId)
    {
        var spec = new ReturnDetailsByUserSpecification(
            returnId,
            userId);

        var returnRequest = await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .GetFirstOrDefaultAsync(spec);

        if (returnRequest is null)
        {
            throw new KeyNotFoundException(
                $"Return request with ID {returnId} not found.");
        }

        return _mapper.Map<ReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<ReturnDetailsResponseDTO>
        CreateReturnAsync(
            string userId,
            CreateReturnRequestDTO dto)
    {
        var validationResult =
            await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ValidationException(
                validationResult.Errors);

        var order = await _unitOfWork
            .GetRepository<Order, Guid>()
            .GetFirstOrDefaultAsync(
                new OrderDetailsSpecification(dto.OrderId));

        if (order is null || order.UserId != userId)
        {
            throw new KeyNotFoundException(
                $"Order with ID {dto.OrderId} not found.");
        }

        var existingReturnItems = await _unitOfWork
            .GetRepository<ReturnItem, Guid>()
            .GetAllAsync(
                new ReturnItemsByOrderSpecification(dto.OrderId));

        // Approved refund policy: refund the actual paid item
        // value after allocated order-level coupon discounts,
        // proportional to the returned quantity. Shipping is
        // never refunded and no fees are added.
        var couponUsages = await _unitOfWork
            .GetRepository<CouponUsage, Guid>()
            .GetAllAsync(
                new CouponUsagesByOrderSpecification(dto.OrderId));

        decimal totalCouponDiscount =
            couponUsages.Sum(cu => cu.DiscountAmount);

        decimal orderSubTotal =
            order.OrderItems.Sum(oi => oi.TotalPrice);

        var returnRequest =
            _mapper.Map<ReturnRequest>(dto);

        returnRequest.UserId = userId;
        returnRequest.OrderId = dto.OrderId;
        returnRequest.Status = ReturnStatus.Requested;
        returnRequest.ReturnNumber = GenerateReturnNumber();
        returnRequest.ReturnItems = new List<ReturnItem>();

        // Quantities and refund values already consumed within
        // this request, so duplicate OrderItemId entries cannot
        // bypass the limits checked against persisted returns.
        var requestedQuantities = new Dictionary<Guid, int>();
        var requestedRefunds = new Dictionary<Guid, decimal>();

        foreach (var item in dto.Items)
        {
            var orderItem = order.OrderItems
                .FirstOrDefault(oi => oi.Id == item.OrderItemId);

            if (orderItem is null)
            {
                throw new ArgumentException(
                    $"Order item {item.OrderItemId} does not belong to order {dto.OrderId}.");
            }

            var alreadyReturned = existingReturnItems
                .Where(ri => ri.OrderItemId == item.OrderItemId)
                .Sum(ri => ri.Quantity);

            requestedQuantities.TryGetValue(
                item.OrderItemId, out var requestedThisRequest);

            var remaining = orderItem.Quantity
                - alreadyReturned
                - requestedThisRequest;

            if (item.Quantity > remaining)
            {
                throw new ArgumentException(
                    $"Requested quantity ({item.Quantity}) for order item {item.OrderItemId} exceeds the returnable quantity ({remaining}).");
            }

            // Cap the cumulative refunded value per line at its
            // net paid value, so repeated partial refunds cannot
            // exceed it through per-request rounding.
            var alreadyRefunded = existingReturnItems
                .Where(ri => ri.OrderItemId == item.OrderItemId)
                .Sum(ri => ri.RefundAmount);

            requestedRefunds.TryGetValue(
                item.OrderItemId, out var refundedThisRequest);

            decimal lineNetValue = CalculateLineNetValue(
                orderItem,
                totalCouponDiscount,
                orderSubTotal);

            decimal remainingRefundable = Math.Max(
                0m,
                lineNetValue - alreadyRefunded - refundedThisRequest);

            decimal refundAmount = Math.Min(
                CalculateRefundAmount(
                    orderItem,
                    item.Quantity,
                    lineNetValue),
                remainingRefundable);

            requestedQuantities[item.OrderItemId] =
                requestedThisRequest + item.Quantity;
            requestedRefunds[item.OrderItemId] =
                refundedThisRequest + refundAmount;

            returnRequest.ReturnItems.Add(new ReturnItem
            {
                OrderItemId = item.OrderItemId,
                ProductId = orderItem.ProductId,
                Quantity = item.Quantity,
                Reason = item.Reason,
                RefundAmount = refundAmount,
                Status = ReturnItemStatus.Pending
            });
        }

        returnRequest.RefundAmount =
            returnRequest.ReturnItems.Sum(ri => ri.RefundAmount);

        // Initial history
        returnRequest.ReturnHistories.Add(
            new ReturnHistory
            {
                OldStatus = null,
                NewStatus = ReturnStatus.Requested,
                Action = "Created",
                PerformedById = userId
            });

        await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .AddAsync(returnRequest);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<ReturnDetailsResponseDTO>
        CancelReturnAsync(
            Guid returnId,
            string userId)
    {
        var spec = new ReturnDetailsByUserSpecification(
            returnId,
            userId);

        var returnRequest = await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .GetFirstOrDefaultAsync(spec);

        if (returnRequest is null)
        {
            throw new KeyNotFoundException(
                $"Return request with ID {returnId} not found.");
        }

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Requested);

        ChangeStatus(
            returnRequest,
            ReturnStatus.Cancelled,
            "Cancelled",
            userId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<ReturnDetailsResponseDTO>(
            returnRequest);
    }


    // =========================================================
    // ADMIN - RETURNS
    // =========================================================

    public async Task<IReadOnlyList<ReturnResponseDTO>>
        GetAllReturnsAsync()
    {
        var returns = await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .GetAllAsync();

        return _mapper.Map<IReadOnlyList<ReturnResponseDTO>>(
            returns);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        GetReturnByIdAsync(Guid returnId)
    {
        var spec = new ReturnDetailsSpecification(returnId);

        var returnRequest = await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .GetFirstOrDefaultAsync(spec);

        if (returnRequest is null)
        {
            throw new KeyNotFoundException(
                $"Return request with ID {returnId} not found.");
        }

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        ApproveReturnAsync(
            Guid returnId,
            string adminId)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Requested);

        ChangeStatus(
            returnRequest,
            ReturnStatus.Approved,
            "Approved",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        RejectReturnAsync(
            Guid returnId,
            string adminId,
            RejectReturnRequestDTO dto)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Requested);

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new ArgumentException(
                "Rejection reason is required.");
        }

        returnRequest.RejectionReason = dto.Reason;

        ChangeStatus(
            returnRequest,
            ReturnStatus.Rejected,
            "Rejected",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        MarkAsInTransitAsync(
            Guid returnId,
            string adminId)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Approved);

        ChangeStatus(
            returnRequest,
            ReturnStatus.InTransit,
            "Marked as In Transit",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        MarkAsReceivedAsync(
            Guid returnId,
            string adminId)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.InTransit);

        ChangeStatus(
            returnRequest,
            ReturnStatus.Received,
            "Marked as Received",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        StartProcessingAsync(
            Guid returnId,
            string adminId)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Received);

        ChangeStatus(
            returnRequest,
            ReturnStatus.Processing,
            "Processing Started",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        ProcessRefundAsync(
            Guid returnId,
            string adminId,
            ProcessRefundRequestDTO dto)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Processing);

        returnRequest.RefundMethod =
            dto.RefundMethod;

        ChangeStatus(
            returnRequest,
            ReturnStatus.Refunded,
            "Refund Processed",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }

    public async Task<AdminReturnDetailsResponseDTO>
        CompleteReturnAsync(
            Guid returnId,
            string adminId)
    {
        var returnRequest =
            await GetReturnAsync(returnId);

        EnsureReturnStatus(
            returnRequest,
            ReturnStatus.Refunded);

        ChangeStatus(
            returnRequest,
            ReturnStatus.Completed,
            "Completed",
            adminId);

        await SaveReturnAsync(returnRequest);

        return _mapper.Map<AdminReturnDetailsResponseDTO>(
            returnRequest);
    }


    // =========================================================
    // ADMIN - RETURN ITEMS
    // =========================================================

    public async Task<ReturnItemResponseDTO>
        ApproveItemAsync(
            Guid returnItemId,
            string adminId)
    {
        var item =
            await GetReturnItemAsync(returnItemId);

        EnsureItemStatus(
            item,
            ReturnItemStatus.Pending);

        item.Status = ReturnItemStatus.Approved;

        await SaveReturnItemAsync(item);

        return _mapper.Map<ReturnItemResponseDTO>(item);
    }

    public async Task<ReturnItemResponseDTO>
        RejectItemAsync(
            Guid returnItemId,
            string adminId)
    {
        var item =
            await GetReturnItemAsync(returnItemId);

        EnsureItemStatus(
            item,
            ReturnItemStatus.Pending);

        item.Status = ReturnItemStatus.Rejected;

        await SaveReturnItemAsync(item);

        return _mapper.Map<ReturnItemResponseDTO>(item);
    }

    public async Task<ReturnItemResponseDTO>
        MarkItemAsReceivedAsync(
            Guid returnItemId,
            string adminId)
    {
        var item =
            await GetReturnItemAsync(returnItemId);

        EnsureItemStatus(
            item,
            ReturnItemStatus.Approved);

        item.Status = ReturnItemStatus.Received;

        await SaveReturnItemAsync(item);

        return _mapper.Map<ReturnItemResponseDTO>(item);
    }

    public async Task<ReturnItemResponseDTO>
        InspectItemAsync(
            Guid returnItemId,
            string adminId)
    {
        var item =
            await GetReturnItemAsync(returnItemId);

        EnsureItemStatus(
            item,
            ReturnItemStatus.Received);

        item.Status = ReturnItemStatus.Inspected;

        await SaveReturnItemAsync(item);

        return _mapper.Map<ReturnItemResponseDTO>(item);
    }

    public async Task<ReturnItemResponseDTO>
        RefundItemAsync(
            Guid returnItemId,
            string adminId)
    {
        var item =
            await GetReturnItemAsync(returnItemId);

        EnsureItemStatus(
            item,
            ReturnItemStatus.Inspected);

        item.Status = ReturnItemStatus.Refunded;

        await SaveReturnItemAsync(item);

        return _mapper.Map<ReturnItemResponseDTO>(item);
    }


    // =========================================================
    // PRIVATE METHODS
    // =========================================================

    private async Task<ReturnRequest>
        GetReturnAsync(Guid returnId)
    {
        var spec =
            new ReturnDetailsSpecification(returnId);

        var returnRequest = await _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .GetFirstOrDefaultAsync(spec);

        if (returnRequest is null)
        {
            throw new KeyNotFoundException(
                $"Return request with ID {returnId} not found.");
        }

        return returnRequest;
    }

    private async Task<ReturnItem>
        GetReturnItemAsync(Guid returnItemId)
    {
        var spec =
            new ReturnItemSpecification(returnItemId);

        var item = await _unitOfWork
            .GetRepository<ReturnItem, Guid>()
            .GetFirstOrDefaultAsync(spec);

        if (item is null)
        {
            throw new KeyNotFoundException(
                $"Return item with ID {returnItemId} not found.");
        }

        return item;
    }

    private static decimal CalculateLineNetValue(
        OrderItem orderItem,
        decimal totalCouponDiscount,
        decimal orderSubTotal)
    {
        // Line totals are gross: the coupon discount is only
        // ever stored as a separate order-level CouponUsage, so
        // subtracting its allocation here cannot double-count a
        // discount already inside TotalPrice.
        decimal lineNet = orderItem.TotalPrice;

        if (totalCouponDiscount > 0 && orderSubTotal > 0)
        {
            // Allocate the order-level discount to this line in
            // proportion to its share of the order subtotal.
            decimal allocatedDiscount = Math.Round(
                totalCouponDiscount *
                (orderItem.TotalPrice / orderSubTotal),
                2,
                MidpointRounding.AwayFromZero);

            lineNet = Math.Max(0m, lineNet - allocatedDiscount);
        }

        return lineNet;
    }

    private static decimal CalculateRefundAmount(
        OrderItem orderItem,
        int returnedQuantity,
        decimal lineNetValue)
    {
        if (orderItem.Quantity <= 0)
            return 0m;

        return Math.Round(
            lineNetValue / orderItem.Quantity * returnedQuantity,
            2,
            MidpointRounding.AwayFromZero);
    }

    private async Task SaveReturnAsync(
        ReturnRequest returnRequest)
    {
        _unitOfWork
            .GetRepository<ReturnRequest, Guid>()
            .Update(returnRequest);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SaveReturnItemAsync(
        ReturnItem item)
    {
        _unitOfWork
            .GetRepository<ReturnItem, Guid>()
            .Update(item);

        await _unitOfWork.SaveChangesAsync();
    }

    private static void EnsureReturnStatus(
        ReturnRequest returnRequest,
        ReturnStatus expectedStatus)
    {
        if (returnRequest.Status != expectedStatus)
        {
            throw new BusinessRuleException(
                $"Return request must be in " +
                $"'{expectedStatus}' status.");
        }
    }

    private static void EnsureItemStatus(
        ReturnItem item,
        ReturnItemStatus expectedStatus)
    {
        if (item.Status != expectedStatus)
        {
            throw new BusinessRuleException(
                $"Return item must be in " +
                $"'{expectedStatus}' status.");
        }
    }

    private static void ChangeStatus(
        ReturnRequest returnRequest,
        ReturnStatus newStatus,
        string action,
        string performedById)
    {
        var oldStatus = returnRequest.Status;

        returnRequest.Status = newStatus;

        AddHistory(
            returnRequest,
            oldStatus,
            newStatus,
            action,
            performedById);
    }

    private static void AddHistory(
        ReturnRequest returnRequest,
        ReturnStatus? oldStatus,
        ReturnStatus newStatus,
        string action,
        string performedById)
    {
        returnRequest.ReturnHistories.Add(
            new ReturnHistory
            {
                ReturnRequestId = returnRequest.Id,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Action = action,
                PerformedById = performedById
            });
    }

    private static string GenerateReturnNumber()
    {
        var uniquePart = Guid.NewGuid()
            .ToString("N")[..8]
            .ToUpperInvariant();

        return $"RET-{DateTime.UtcNow:yyyyMMddHHmmss}-{uniquePart}";
    }
}
