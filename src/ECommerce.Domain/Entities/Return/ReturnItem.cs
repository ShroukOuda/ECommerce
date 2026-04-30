using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Return;

namespace ECommerce.Domain.Entities.Return;

public class ReturnItem : BaseEntity<Guid>
{
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public ReturnItemStatus Status { get; set; } = ReturnItemStatus.Pending;
    
    //FK
    public Guid ReturnRequestId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    
    //Navigation Properties
    public virtual ReturnRequest? ReturnRequest { get; set; }
    public virtual Order.OrderItem? OrderItem { get; set; }
    public virtual Product.Product? Product { get; set; }
}