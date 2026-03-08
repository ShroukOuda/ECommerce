using ECommerce.Core.Entities.Return;
using ECommerce.Core.Enums.Return;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ReturnSeed
{
    private static readonly DateTime BaseDate = new(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly string[] Reasons =
    [
        "Product defective", "Wrong item received", "Item damaged during shipping",
        "Does not match description", "Changed my mind", "Found a better price",
        "Size does not fit", "Wrong color", "Missing accessories",
        "Product stopped working", "Quality not as expected", "Arrived too late"
    ];

    public static void SeedReturns(ModelBuilder modelBuilder)
    {
        var returnRequests = new List<ReturnRequest>();
        var returnItems = new List<ReturnItem>();
        int returnItemId = 1;

        // 50 returns from delivered orders (orders 1-200)
        for (int returnId = 1; returnId <= 50; returnId++)
        {
            var orderId = returnId * 4; // orders 4, 8, 12, ..., 200
            var userIndex = ((orderId - 1) % 180) + 16;
            var userId = UserSeed.GetUserId(userIndex);
            var orderDate = BaseDate.AddDays((orderId * 33) % 540).AddHours((orderId * 7) % 24);
            var requestDate = orderDate.AddDays(5 + (returnId % 10));

            ReturnStatus status;
            DateTime? approvedDate = null;
            DateTime? completedDate = null;
            DateTime? refundDate = null;
            string? refundMethod = null;

            if (returnId <= 20) // Completed/Refunded
            {
                status = ReturnStatus.Refunded;
                approvedDate = requestDate.AddDays(1);
                completedDate = requestDate.AddDays(7);
                refundDate = requestDate.AddDays(10);
                refundMethod = "Original Payment Method";
            }
            else if (returnId <= 30) // Approved, in process
            {
                status = ReturnStatus.Approved;
                approvedDate = requestDate.AddDays(1);
            }
            else if (returnId <= 35) // In Transit back
            {
                status = ReturnStatus.InTransit;
                approvedDate = requestDate.AddDays(1);
            }
            else if (returnId <= 40) // Received, inspecting
            {
                status = ReturnStatus.Received;
                approvedDate = requestDate.AddDays(1);
            }
            else if (returnId <= 45) // Requested, awaiting approval
            {
                status = ReturnStatus.Requested;
            }
            else // Rejected
            {
                status = ReturnStatus.Rejected;
                approvedDate = null; // not approved
            }

            var reason = Reasons[(returnId - 1) % Reasons.Length];
            var itemCount = (returnId % 3) + 1; // 1-3 items per return

            decimal totalRefund = 0m;

            for (int item = 0; item < itemCount; item++)
            {
                var productId = ((orderId * 3 + item * 7) % 80) + 1;
                decimal refundAmount = productId switch
                {
                    <= 5 => 999.99m,
                    <= 20 => 399.99m,
                    <= 35 => 699.99m,
                    <= 50 => 149.99m,
                    <= 65 => 599.99m,
                    _ => 199.99m
                };

                var itemStatus = status switch
                {
                    ReturnStatus.Refunded => ReturnItemStatus.Refunded,
                    ReturnStatus.Received => ReturnItemStatus.Received,
                    ReturnStatus.Approved or ReturnStatus.InTransit => ReturnItemStatus.Approved,
                    ReturnStatus.Rejected => ReturnItemStatus.Rejected,
                    _ => ReturnItemStatus.Pending
                };

                totalRefund += refundAmount;

                // OrderItemId: approximate from orderId and item index
                var orderItemId = ((orderId - 1) * 3 + item + 1);
                if (orderItemId > 1800) orderItemId = 1;

                returnItems.Add(new ReturnItem
                {
                    Id = returnItemId++,
                    ReturnRequestId = returnId,
                    OrderItemId = orderItemId,
                    ProductId = productId,
                    Quantity = 1,
                    Reason = reason,
                    RefundAmount = refundAmount,
                    Status = itemStatus,
                    CreatedAt = requestDate,
                    UpdatedAt = completedDate ?? approvedDate ?? requestDate,
                    IsDeleted = false
                });
            }

            returnRequests.Add(new ReturnRequest
            {
                Id = returnId,
                OrderId = orderId,
                UserId = userId,
                ReturnNumber = $"RET-{returnId:D6}",
                Reason = reason,
                Description = $"Return request for order #{orderId}: {reason}",
                Status = status,
                RefundAmount = totalRefund,
                RefundMethod = refundMethod,
                RefundDate = refundDate,
                RequestedDate = requestDate,
                ApprovedDate = approvedDate,
                CompletedDate = completedDate,
                CreatedAt = requestDate,
                UpdatedAt = completedDate ?? approvedDate ?? requestDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<ReturnRequest>().HasData(returnRequests.ToArray());
        modelBuilder.Entity<ReturnItem>().HasData(returnItems.ToArray());
    }
}
