using ECommerce.Application.DTO.Address;

namespace ECommerce.Application.Interfaces;

public interface IAddressService
{
    Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(string userId);
    Task<GetAddressDTO> GetAddressByIdAsync(Guid id);
    Task<GetAddressDTO> AddAddressAsync(AddAddressDTO dto);
    Task<GetAddressDTO> UpdateAddressAsync(Guid id, UpdateAddressDTO dto);
    Task DeleteAddressAsync(Guid id);
}
