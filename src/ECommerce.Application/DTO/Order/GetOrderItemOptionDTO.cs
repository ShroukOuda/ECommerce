namespace ECommerce.Application.DTO.Order;

public class GetOrderItemOptionDTO
{
    public Guid Id { get; set; }

    public string OptionName { get; set; } = string.Empty;

    public string OptionValue { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }
}