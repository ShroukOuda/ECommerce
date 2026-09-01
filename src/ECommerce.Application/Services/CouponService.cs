using ECommerce.Application.DTO.Coupon;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Coupons;

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
        var coupons = await _unitOfWork.GetRepository<Coupon, Guid>().GetAllAsync(ct);
        return _mapper.Map<IEnumerable<GetCouponDTO>>(coupons);
    }

    public async Task<GetCouponDTO> GetCouponByIdAsync(Guid id, CancellationToken ct = default)
    {
        var coupon = await _unitOfWork.GetRepository<Coupon, Guid>().GetByIdAsync(id, ct);
        if (coupon is null) throw new KeyNotFoundException($"Coupon with ID {id} not found.");
        return _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task<GetCouponDTO?> GetCouponByCodeAsync(string code, CancellationToken ct = default)
    {
        var spec = new CouponByCodeSpecification(code);
        var coupon = await _unitOfWork.GetRepository<Coupon, Guid>().GetFirstOrDefaultAsync(spec);
        return coupon is null ? null : _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task<GetCouponDTO> AddCouponAsync(AddCouponDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var coupon = _mapper.Map<Coupon>(dto);
        await _unitOfWork.GetRepository<Coupon, Guid>().AddAsync(coupon, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task<GetCouponDTO> UpdateCouponAsync(Guid id, UpdateCouponDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var spec = new CouponSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Coupon, Guid>().ExistsAsync(spec, ct);
        if (!exists) throw new KeyNotFoundException($"Coupon with ID {id} not found.");
        var coupon = _mapper.Map<Coupon>(dto);
        _unitOfWork.GetRepository<Coupon, Guid>().Update(coupon, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task DeleteCouponAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new CouponSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Coupon, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Coupon with ID {id} not found.");
        var stub = new Coupon { Id = id };
        _unitOfWork.GetRepository<Coupon, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
