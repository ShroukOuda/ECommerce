using ECommerce.Application.DTO.Return;
using ECommerce.Core.Entities.Return;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class ReturnService : IReturnService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateReturnRequestDTO> _createValidator;

    public ReturnService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateReturnRequestDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetReturnRequestDTO>> GetReturnsByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var returns = await _unitOfWork.ReturnRequestRepository.GetReturnsByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<GetReturnRequestDTO>>(returns);
    }

    public async Task<GetReturnRequestDTO> GetReturnByIdAsync(int id, CancellationToken ct = default)
    {
        var returnReq = await _unitOfWork.ReturnRequestRepository.GetReturnWithItemsAsync(id, ct);
        if (returnReq is null) throw new KeyNotFoundException($"Return request with ID {id} not found.");
        return _mapper.Map<GetReturnRequestDTO>(returnReq);
    }

    public async Task<GetReturnRequestDTO> CreateReturnRequestAsync(CreateReturnRequestDTO dto, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var returnReq = new ReturnRequest
        {
            OrderId = dto.OrderId,
            UserId = dto.UserId,
            Reason = dto.Reason,
            Description = dto.Description,
            ReturnNumber = $"RET-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            RequestedDate = DateTime.UtcNow
        };

        await _unitOfWork.ReturnRequestRepository.AddAsync(returnReq, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetReturnRequestDTO>(returnReq);
    }
}
