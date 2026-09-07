using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.DTO.Return;

public class ReturnItemResponseDTO
{
    public Guid Id { get; set; }

    public Guid OrderItemId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string Reason { get; set; } = string.Empty;

    public decimal RefundAmount { get; set; }

    public ReturnItemStatus Status { get; set; }
}