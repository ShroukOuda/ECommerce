using ECommerce.Application.DTO.Brand;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Brands;

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

    public async Task<IEnumerable<GetBrandDTO>> GetAllBrandsAsync()
    {
        var brands = await _unitOfWork.GetRepository<Brand, Guid>().GetAllAsync();
        return _mapper.Map<IEnumerable<GetBrandDTO>>(brands);
    }

    public async Task<GetBrandDTO> GetBrandByIdAsync(Guid id)
    {
        var brand = await _unitOfWork.GetRepository<Brand, Guid>().GetByIdAsync(id);
        if (brand is null) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        return _mapper.Map<GetBrandDTO>(brand);
    }

    public async Task<GetBrandDTO> AddBrandAsync(AddBrandDTO dto)
    {
        var result = await _addValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var brand = _mapper.Map<Brand>(dto);
        brand.Slug = dto.Name.ToLower().Replace(" ", "-");
        await _unitOfWork.GetRepository<Brand, Guid>().AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetBrandDTO>(brand);
    }

    public async Task<GetBrandDTO> UpdateBrandAsync(Guid id, UpdateBrandDTO dto)
    {

        var result = await _updateValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var spec = new BrandSpecification(id);
        var existingBrand = await _unitOfWork.GetRepository<Brand, Guid>().ExistsAsync(spec);
        if (!existingBrand) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        
        var brand = _mapper.Map<Brand>(dto);
        brand.Slug = dto.Name.ToLower().Replace(" ", "-");
        _unitOfWork.GetRepository<Brand, Guid>().Update(brand);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetBrandDTO>(brand);
    }

    public async Task DeleteBrandAsync(Guid id)
    {
        var spec = new BrandSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Brand, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        var stub = new Brand { Id = id };
        _unitOfWork.GetRepository<Brand, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
    }
}
