namespace ECommerce.Domain.Enums.Notification;

public enum NotificationType
{
    // Security
    SecurityAlert,
    LoginFromNewDevice,
    PasswordChanged,

    // Products
    NewProduct,
    BackInStock,

    // Orders
    OrderPlaced,
    OrderShipped,
    OrderDelivered,
    OrderCancelled,

    // Promotions
    Promotion
}