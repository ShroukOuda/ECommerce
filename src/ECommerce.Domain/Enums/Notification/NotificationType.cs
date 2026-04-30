namespace ECommerce.Domain.Enums.Notification;

public enum NotificationType
{
    OrderConfirmation = 1,
    OrderShipped = 2,
    OrderDelivered = 3,
    PaymentSuccess = 4,
    PaymentFailed = 5,
    LowStock = 6,
    PriceDrop = 7,
    NewProduct = 8,
    AbandonedCart = 9,
    ReviewReminder = 10,
    Newsletter = 11
}