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

    public async Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(Guid productId)
    {
        var spec = new ProductOptionsByProductSpecification(productId);
        var options = await _unitOfWork.GetRepository<ProductOption, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetProductOptionDTO>>(options);
    }

    public async Task<GetProductOptionDTO> GetOptionByIdAsync(Guid id)
    {
        var option = await _unitOfWork.GetRepository<ProductOption, Guid>().GetByIdAsync(id);
        if (option is null) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        return _mapper.Map<GetProductOptionDTO>(option);
    }

    public async Task<GetProductOptionDTO> AddOptionAsync(AddProductOptionDTO dto)
    {
        var result = await _addValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var option = _mapper.Map<ProductOption>(dto);
        await _unitOfWork.GetRepository<ProductOption, Guid>().AddAsync(option);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetProductOptionDTO>(option);
    }

    public async Task<GetProductOptionDTO> UpdateOptionAsync(Guid id, UpdateProductOptionDTO dto)
    {
        var result = await _updateValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var spec = new ProductOptionSpecification(id);
        bool exists = await _unitOfWork.GetRepository<ProductOption, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        var option = _mapper.Map<ProductOption>(dto);
        _unitOfWork.GetRepository<ProductOption, Guid>().Update(option);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetProductOptionDTO>(option);
    }

    public async Task DeleteOptionAsync(Guid id)
    {
        var spec = new ProductOptionSpecification(id);
        bool exists = await _unitOfWork.GetRepository<ProductOption, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product option with ID {id} not found.");
        var stub = new ProductOption { Id = id };
        _unitOfWork.GetRepository<ProductOption, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetProductOptionValueDTO> AddOptionValueAsync(Guid optionId, AddProductOptionValueDTO dto)
    {
        var result = await _addValueValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var spec = new ProductOptionSpecification(optionId);
        bool exists = await _unitOfWork.GetRepository<ProductOption, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product option with ID {optionId} not found.");
        
        var value = _mapper.Map<ProductOptionValue>(dto);
        await _unitOfWork.GetRepository<ProductOptionValue, Guid>().AddAsync(value);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetProductOptionValueDTO>(value);
    }

    public async Task DeleteOptionValueAsync(Guid optionId, Guid valueId)
    {
        var spec = new ProductOptionValueSpecification(optionId, valueId);
        bool exists = await _unitOfWork.GetRepository<ProductOptionValue, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product option value with ID {valueId} not found.");
        var stub = new ProductOptionValue { Id = valueId };
        _unitOfWork.GetRepository<ProductOptionValue, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
    }
}
