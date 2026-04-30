using ECommerce.Application.DTO.Address;

namespace ECommerce.API.Controllers;

public class AddressesController : BaseController
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
        return Ok(addresses);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);
        return Ok(address);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddAddressDTO dto)
    {
        await _addressService.AddAddressAsync(dto);
        return Ok(new ResponseAPI(200, "Address added successfully"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateAddressDTO dto)
    {
        await _addressService.UpdateAddressAsync(dto);
        return Ok(new ResponseAPI(200, "Address updated successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _addressService.DeleteAddressAsync(id);
        return Ok(new ResponseAPI(200, "Address deleted successfully"));
    }
}
