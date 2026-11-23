using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums.Return;

namespace ECommerce.Domain.Entities.Returns;

public class ReturnRequest : BaseEntity<Guid>
{
    public string ReturnNumber { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string RejectionReason { get; set; } = string.Empty;

    public ReturnStatus Status { get; set; } = ReturnStatus.Requested;

    public decimal RefundAmount { get; set; }

    public RefundMethod? RefundMethod { get; set; }


    // Foreign Keys
    public Guid OrderId { get; set; }

    //Customer who requested the return
    public string UserId { get; set; } = null!;

    // Navigation Properties
    public Order Order { get; set; } = null!;

    public User User { get; set; } = null!;

    public ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    public ICollection<ReturnHistory> ReturnHistories { get; set; } = new List<ReturnHistory>();
}
