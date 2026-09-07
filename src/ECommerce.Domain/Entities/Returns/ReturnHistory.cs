
using ECommerce.Domain.Enums.Return;

namespace ECommerce.Domain.Entities.Returns;

public class ReturnHistory : BaseEntity<Guid>
{
    public Guid ReturnRequestId { get; set; }

    public ReturnStatus? OldStatus { get; set; }

    public ReturnStatus NewStatus { get; set; }

    public string Action { get; set; } = string.Empty;

    //Admin or user who performed the action
    public string PerformedById { get; set; } = null!;


    // Navigation Properties
    public ReturnRequest ReturnRequest { get; set; } = null!;

    public User PerformedBy { get; set; } = null!;
}