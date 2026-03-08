namespace ECommerce.Application.DTO.Shipping;

public class CreateShippingDTO
{
    public int OrderId { get; set; }
    public int AddressId { get; set; }
    public string Method { get; set; } = "Standard";
    public decimal Cost { get; set; }
}
