using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.DTO.Return;

public class ReturnResponseDTO
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public ReturnStatus Status { get; set; }
    public decimal RefundAmount { get; set; }
    public DateTime RequestedDate { get; set; }
    public int ItemCount { get; set; }
}