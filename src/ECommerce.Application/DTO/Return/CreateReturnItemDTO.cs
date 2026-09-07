namespace ECommerce.Application.DTO.Return;

public class CreateReturnItemDTO
{
    public Guid OrderItemId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
