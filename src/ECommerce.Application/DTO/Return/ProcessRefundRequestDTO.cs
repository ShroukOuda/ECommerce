using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.DTO.Return;

public class ProcessRefundRequestDTO
{
    public RefundMethod RefundMethod { get; set; }
}