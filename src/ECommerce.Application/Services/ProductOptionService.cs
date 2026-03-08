using ECommerce.Application.DTO.ProductOption;
using ECommerce.Core.Entities.Product;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class ProductOptionService : IProductOptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddProductOptionDTO> _addValidator;
    private readonly IValidator<UpdateProductOptionDTO> _updateValidator;
    private readonly IValidator<AddProductOptionValueDTO> _addValueValidator;

    public ProductOptionService(IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<AddProductOptionDTO> addValidator,
        IValidator<UpdateProductOptionDTO> updateValidator,
        IValidator<AddProductOptionValueDTO> addValueValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
        _addValueValidator = addValueValidator;
    }

    public async Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(int productId, CancellationToken ct = default)
    {
        var options = await _unitOfWork.ProductOptionRepository.GetOptionsByProductIdAsync(productId, ct);
        return _mapper.Map<IEnumerable<GetProductOptionDTO>>(options);
    }

    public async Task<GetProductOptionDTO> GetOptionByIdAsync(int id, CancellationToken ct = default)
    {
        var option = await _unitOfWork.ProductOptionRepository.GetByIdAsync(id, ct);
        if (option is null) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        return _mapper.Map<GetProductOptionDTO>(option);
    }

    public async Task AddOptionAsync(AddProductOptionDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var option = _mapper.Map<ProductOption>(dto);
        await _unitOfWork.ProductOptionRepository.AddAsync(option, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateOptionAsync(UpdateProductOptionDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var option = _mapper.Map<ProductOption>(dto);
        await _unitOfWork.ProductOptionRepository.UpdateAsync(option, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOptionAsync(int id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.ProductOptionRepository.ExistsAsync(o => o.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        var stub = new ProductOption { Id = id };
        await _unitOfWork.ProductOptionRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AddOptionValueAsync(AddProductOptionValueDTO dto, CancellationToken ct = default)
    {
        var result = await _addValueValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var value = _mapper.Map<ProductOptionValue>(dto);
        await _unitOfWork.ProductOptionValueRepository.AddAsync(value, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOptionValueAsync(int id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.ProductOptionValueRepository.ExistsAsync(v => v.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Product option value with ID {id} not found.");
        var stub = new ProductOptionValue { Id = id };
        await _unitOfWork.ProductOptionValueRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
