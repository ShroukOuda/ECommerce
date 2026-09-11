namespace ECommerce.Application.DTO.Order;

public class CreateOrderItemOptionDTO
{
    public Guid ProductOptionId { get; set; }

    public string OptionName { get; set; } = string.Empty;

    public string OptionValue { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }
}