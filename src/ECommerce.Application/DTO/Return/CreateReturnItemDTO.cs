namespace ECommerce.Application.DTO.Return;

public class CreateReturnItemDTO
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
