using ECommerce.Application.DTO.Shipping;
using ECommerce.Domain.Entities.Shippings;
using ECommerce.Domain.Enums.Shipping;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Shippings;

namespace ECommerce.Application.Services;

public class ShippingService : IShippingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateShippingDTO> _createValidator;

    public ShippingService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateShippingDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetShippingDTO>> GetMyShippingsAsync(string userId)
    {
        var spec = new ShippingsByUserSpecification(userId);
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<GetShippingDTO> GetMyShippingByIdAsync(Guid shippingId, string userId)
    {
        var spec = new ShippingsByUserSpecification(shippingId, userId);
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(spec);
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {shippingId} not found for user {userId}.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<GetShippingDTO> GetMyShippingByOrderIdAsync(Guid orderId, string userId)
    {
        var spec = new ShippingsByOrderSpecification(orderId, userId);
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(spec);
        if (shipping is null) throw new KeyNotFoundException($"Shipping for order ID {orderId} not found for user {userId}.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<IEnumerable<GetShippingDTO>> GetAllShippingsAsync()
    {
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync();
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<IEnumerable<GetShippingDTO>> GetShippingsByOrderIdAsync(Guid orderId)
    {
        var spec = new ShippingsByOrderSpecification(orderId);
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<GetShippingDTO> GetShippingByIdAsync(Guid id)
    {
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetByIdAsync(id);
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {id} not found.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<GetShippingDTO> CreateShippingAsync(CreateShippingDTO dto)
    {
        var result = await _createValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var shipping = _mapper.Map<Shipping>(dto);

        await _unitOfWork.GetRepository<Shipping, Guid>().AddAsync(shipping);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<GetShippingDTO> UpdateShippingAsync(Guid shippingId, UpdateShippingDTO dto)
    {
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetByIdAsync(shippingId);
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {shippingId} not found.");
        _mapper.Map(dto, shipping);
        _unitOfWork.GetRepository<Shipping, Guid>().Update(shipping);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetShippingDTO>(shipping);
    }
}
