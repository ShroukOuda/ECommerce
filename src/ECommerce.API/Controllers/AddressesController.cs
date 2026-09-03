using ECommerce.Application.DTO.Address;

namespace ECommerce.API.Controllers;

public class AddressesController : BaseController
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet()]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> GetMyAddresses()
    {
        var addresses = await _addressService.GetAddressesByUserIdAsync(CurrentUserId);
        return Success(
            addresses,
            "Addresses retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);
        return Success(
            address,
            "Address retrieved successfully.");
    }

    [HttpPost()]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> Add(AddAddressDTO dto)
    {
        var address = await _addressService.AddAddressAsync(dto);
        return Created(address, "Address added successfully.");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> Update(Guid id, UpdateAddressDTO dto)
    {
        var address = await _addressService.UpdateAddressAsync(id, dto);
        return Success(address, "Address updated successfully");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _addressService.DeleteAddressAsync(id);
        return NoContent();
    }
}
