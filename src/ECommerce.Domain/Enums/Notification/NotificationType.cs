namespace ECommerce.Domain.Enums.Notification;

public enum NotificationType
{
    // Orders
    OrderPlaced,
    OrderConfirmed,
    OrderShipped,
    OrderDelivered,
    OrderCancelled,

    // Products
    BackInStock,
    PriceDrop,
    NewProduct,

    // Reviews
    ReviewReply,

    // Promotions
    Promotion,
    Coupon,

    // Security
    LoginAlert,
    PasswordChanged,
    SecurityAlert,

    // System
    SystemAnnouncement
}