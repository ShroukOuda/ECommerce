namespace ECommerce.Application.DTO.Return;

public class GetReturnRequestDTO
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public List<GetReturnItemDTO> Items { get; set; } = new();
}
