namespace ECommerce.Application.DTO.Shipping;

public class CreateShippingDTO
{
    public Guid OrderId { get; set; }
    public Guid AddressId { get; set; }
    public string Method { get; set; } = "Standard";
    public decimal Cost { get; set; }
}
