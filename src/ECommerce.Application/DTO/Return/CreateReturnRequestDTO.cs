namespace ECommerce.Application.DTO.Return;

public class CreateReturnRequestDTO
{
    public Guid OrderId { get; set; }
    public string? Description { get; set; }
    public List<CreateReturnItemDTO> Items { get; set; } = new();
}
