using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.DTO.Return;

public class ReturnDetailsResponseDTO
{
    public Guid Id { get; set; }

    public string ReturnNumber { get; set; } = string.Empty;

    public Guid OrderId { get; set; }


    public ReturnStatus Status { get; set; }

    public string? Description { get; set; }

    public string? RejectionReason { get; set; }

    public decimal RefundAmount { get; set; }

    public RefundMethod? RefundMethod { get; set; }

    public DateTime RequestedDate { get; set; }

    public List<ReturnItemResponseDTO> Items { get; set; } = [];

    public List<ReturnHistoryResponseDTO> History { get; set; } = [];
}