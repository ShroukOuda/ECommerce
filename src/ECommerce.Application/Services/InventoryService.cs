using ECommerce.Application.DTO.Inventory;
using ECommerce.Domain.Entities.Inventory;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInventoryHistoryDTO> _createValidator;

    public InventoryService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateInventoryHistoryDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetInventoryHistoryDTO>> GetHistoryByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var history = await _unitOfWork.InventoryHistoryRepository.GetHistoryByProductIdAsync(productId, ct);
        return _mapper.Map<IEnumerable<GetInventoryHistoryDTO>>(history);
    }

    public async Task AddInventoryHistoryAsync(CreateInventoryHistoryDTO dto, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var history = _mapper.Map<InventoryHistory>(dto);
        await _unitOfWork.InventoryHistoryRepository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
