using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Order;

public class OrderItemOption : BaseEntity<Guid>
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    
    //FK
    public Guid OrderItemId { get; set; }
    
    //Navigation Properties
    public virtual OrderItem? OrderItem { get; set; }
}