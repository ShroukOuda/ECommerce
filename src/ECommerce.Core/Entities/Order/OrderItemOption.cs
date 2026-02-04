namespace ECommerce.Core.Entities.Order;

public class OrderItemOption : BaseEntity<int>
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    
    //FK
    public int OrderItemId { get; set; }
    
    //Navigation Properties
    public virtual OrderItem? OrderItem { get; set; }
}