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

    public async Task<IEnumerable<GetShippingDTO>> GetShippingsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var spec = new ShippingsByOrderSpecification(orderId);
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<GetShippingDTO> GetShippingByIdAsync(Guid id, CancellationToken ct = default)
    {
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetByIdAsync(id, ct);
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {id} not found.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<GetShippingDTO> CreateShippingAsync(CreateShippingDTO dto, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var shipping = new Shipping
        {
            OrderId = dto.OrderId,
            AddressId = dto.AddressId,
            Cost = dto.Cost,
            Method = Enum.Parse<ShippingMethod>(dto.Method),
            TrackingNumber = $"SHP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Status = ShippingStatus.Pending
        };

        await _unitOfWork.GetRepository<Shipping, Guid>().AddAsync(shipping, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetShippingDTO>(shipping);
    }
}
