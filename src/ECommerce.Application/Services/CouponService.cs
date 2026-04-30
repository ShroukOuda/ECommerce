using ECommerce.Application.DTO.Coupon;
using ECommerce.Domain.Entities.Coupon;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddCouponDTO> _addValidator;
    private readonly IValidator<UpdateCouponDTO> _updateValidator;

    public CouponService(IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<AddCouponDTO> addValidator, IValidator<UpdateCouponDTO> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<GetCouponDTO>> GetAllCouponsAsync(CancellationToken ct = default)
    {
        var coupons = await _unitOfWork.CouponRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<GetCouponDTO>>(coupons);
    }

    public async Task<GetCouponDTO> GetCouponByIdAsync(Guid id, CancellationToken ct = default)
    {
        var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id, ct);
        if (coupon is null) throw new KeyNotFoundException($"Coupon with ID {id} not found.");
        return _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task<GetCouponDTO?> GetCouponByCodeAsync(string code, CancellationToken ct = default)
    {
        var coupon = await _unitOfWork.CouponRepository.GetByCodeAsync(code, ct);
        return coupon is null ? null : _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task AddCouponAsync(AddCouponDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var coupon = _mapper.Map<Coupon>(dto);
        await _unitOfWork.CouponRepository.AddAsync(coupon, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateCouponAsync(UpdateCouponDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var coupon = _mapper.Map<Coupon>(dto);
        await _unitOfWork.CouponRepository.UpdateAsync(coupon, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteCouponAsync(Guid id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.CouponRepository.ExistsAsync(c => c.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Coupon with ID {id} not found.");
        var stub = new Coupon { Id = id };
        await _unitOfWork.CouponRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
