 namespace ECommerce.Domain.Enums.Payment;

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Paid = 3,
    Failed = 4,
    Expired = 5,
    Refunded = 6
}