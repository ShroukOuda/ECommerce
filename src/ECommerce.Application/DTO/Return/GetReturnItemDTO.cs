namespace ECommerce.Application.DTO.Return;

public class GetReturnItemDTO
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
