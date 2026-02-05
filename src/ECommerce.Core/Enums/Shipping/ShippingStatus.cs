namespace ECommerce.Core.Enums.Shipping;

public enum ShippingStatus
{
    Pending = 1,
    LabelCreated = 2,
    InTransit = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Failed = 6,
    Returned = 7
}