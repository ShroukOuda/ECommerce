using ECommerce.Domain.Enums.Return;

namespace ECommerce.Domain.Entities.Returns;

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
    public  ReturnRequest ReturnRequest { get; set; } = null!;
    public  OrderItem OrderItem { get; set; } = null!;
    public  Product Product { get; set; } = null!;
}