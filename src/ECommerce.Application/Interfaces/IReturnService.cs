using ECommerce.Application.DTO.Return;

namespace ECommerce.Application.Interfaces;

public interface IReturnService
{
    Task<IEnumerable<GetReturnRequestDTO>> GetReturnsByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GetReturnRequestDTO> GetReturnByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetReturnRequestDTO> CreateReturnRequestAsync(CreateReturnRequestDTO dto, CancellationToken ct = default);
}
