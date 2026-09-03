using ECommerce.Application.DTO.Return;

namespace ECommerce.Application.Interfaces;

public interface IReturnService
{
    Task<IEnumerable<GetReturnRequestDTO>> GetReturnsByUserIdAsync(string userId);
    Task<GetReturnRequestDTO> GetReturnByIdAsync(Guid id);
    Task<GetReturnRequestDTO> CreateReturnRequestAsync(CreateReturnRequestDTO dto);
}
