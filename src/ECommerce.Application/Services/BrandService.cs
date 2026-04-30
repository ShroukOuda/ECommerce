using ECommerce.Application.DTO.Brand;
using ECommerce.Domain.Entities.Brand;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddBrandDTO> _addValidator;
    private readonly IValidator<UpdateBrandDTO> _updateValidator;

    public BrandService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<AddBrandDTO> addValidator,
        IValidator<UpdateBrandDTO> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<GetBrandDTO>> GetAllBrandsAsync(CancellationToken ct = default)
    {
        var brands = await _unitOfWork.BrandRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<GetBrandDTO>>(brands);
    }

    public async Task<GetBrandDTO> GetBrandByIdAsync(Guid id, CancellationToken ct = default)
    {
        var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id, ct);
        if (brand is null) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        return _mapper.Map<GetBrandDTO>(brand);
    }

    public async Task AddBrandAsync(AddBrandDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var brand = _mapper.Map<Brand>(dto);
        brand.Slug = dto.Name.ToLower().Replace(" ", "-");
        await _unitOfWork.BrandRepository.AddAsync(brand, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateBrandAsync(UpdateBrandDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var brand = _mapper.Map<Brand>(dto);
        brand.Slug = dto.Name.ToLower().Replace(" ", "-");
        await _unitOfWork.BrandRepository.UpdateAsync(brand, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteBrandAsync(Guid id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.BrandRepository.ExistsAsync(b => b.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        var stub = new Brand { Id = id };
        await _unitOfWork.BrandRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
