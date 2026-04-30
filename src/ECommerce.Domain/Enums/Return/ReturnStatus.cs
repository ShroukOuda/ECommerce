namespace ECommerce.Domain.Enums.Return;

public enum ReturnStatus
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    InTransit = 4,
    Received = 5,
    Processing = 6,
    Refunded = 7,
    Completed = 8,
    Cancelled = 9
}