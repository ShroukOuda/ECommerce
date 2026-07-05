using ECommerce.Application.DTO.ProductOption;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Products;

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

    public async Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var spec = new ProductOptionsByProductSpecification(productId);
        var options = await _unitOfWork.GetRepository<ProductOption, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetProductOptionDTO>>(options);
    }

    public async Task<GetProductOptionDTO> GetOptionByIdAsync(Guid id, CancellationToken ct = default)
    {
        var option = await _unitOfWork.GetRepository<ProductOption, Guid>().GetByIdAsync(id, ct);
        if (option is null) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        return _mapper.Map<GetProductOptionDTO>(option);
    }

    public async Task AddOptionAsync(AddProductOptionDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var option = _mapper.Map<ProductOption>(dto);
        await _unitOfWork.GetRepository<ProductOption, Guid>().AddAsync(option, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateOptionAsync(UpdateProductOptionDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var option = _mapper.Map<ProductOption>(dto);
        _unitOfWork.GetRepository<ProductOption, Guid>().Update(option, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOptionAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new ProductOptionSpecification(id);
        bool exists = await _unitOfWork.GetRepository<ProductOption, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        var stub = new ProductOption { Id = id };
        _unitOfWork.GetRepository<ProductOption, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AddOptionValueAsync(AddProductOptionValueDTO dto, CancellationToken ct = default)
    {
        var result = await _addValueValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var value = _mapper.Map<ProductOptionValue>(dto);
        await _unitOfWork.GetRepository<ProductOptionValue, Guid>().AddAsync(value, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOptionValueAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new ProductOptionValueSpecification(id);
        bool exists = await _unitOfWork.GetRepository<ProductOptionValue, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product option value with ID {id} not found.");
        var stub = new ProductOptionValue { Id = id };
        _unitOfWork.GetRepository<ProductOptionValue, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
