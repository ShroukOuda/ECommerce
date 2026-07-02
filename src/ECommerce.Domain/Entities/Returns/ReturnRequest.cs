using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Return;

namespace ECommerce.Domain.Entities.Returns;

public class ReturnRequest : BaseEntity<Guid>
{
    public string ReturnNumber { get; set; } = string.Empty; 
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReturnStatus Status { get; set; } = ReturnStatus.Requested;  
    
    public decimal RefundAmount { get; set; }
    public string? RefundMethod { get; set; }
    public DateTime? RefundDate { get; set; }
    
    public DateTime RequestedDate { get; set; } = DateTime.Now;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    //FK
    public Guid OrderId { get; set; }
    public string UserId { get; set; }
    
    //Navigation Properties
    public virtual Orders.Order? Order { get; set; }
    public virtual Users.User? User { get; set; }
    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}