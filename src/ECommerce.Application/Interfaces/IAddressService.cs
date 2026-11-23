using ECommerce.Application.DTO.Address;

namespace ECommerce.Application.Interfaces;

public interface IAddressService
{
    Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(string userId);
    Task<GetAddressDTO> GetAddressByIdAsync(Guid id, string userId);
    Task<GetAddressDTO> AddAddressAsync(AddAddressDTO dto, string userId);
    Task<GetAddressDTO> UpdateAddressAsync(Guid id, UpdateAddressDTO dto, string userId);
    Task DeleteAddressAsync(Guid id, string userId);
}
