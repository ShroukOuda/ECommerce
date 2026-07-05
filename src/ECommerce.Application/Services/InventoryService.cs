using ECommerce.Application.DTO.Inventory;
using ECommerce.Domain.Entities.Inventories;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Inventories;

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
        var spec = new InventoryHistoryByProductSpecification(productId);
        var history = await _unitOfWork.GetRepository<InventoryHistory, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetInventoryHistoryDTO>>(history);
    }

    public async Task AddInventoryHistoryAsync(CreateInventoryHistoryDTO dto, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var history = _mapper.Map<InventoryHistory>(dto);
        await _unitOfWork.GetRepository<InventoryHistory, Guid>().AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
