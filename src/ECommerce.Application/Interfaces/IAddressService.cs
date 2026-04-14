using ECommerce.Application.DTO.Address;

namespace ECommerce.Application.Interfaces;

public interface IAddressService
{
    Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GetAddressDTO> GetAddressByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAddressAsync(AddAddressDTO dto, CancellationToken ct = default);
    Task UpdateAddressAsync(UpdateAddressDTO dto, CancellationToken ct = default);
    Task DeleteAddressAsync(Guid id, CancellationToken ct = default);
}
