namespace ECommerce.Application.DTO.Address;

public class GetAddressDTO : AddressBaseDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
