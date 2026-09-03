using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Products;

namespace ECommerce.Application.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddProductVariantDTO> _addValidator;
    private readonly IValidator<UpdateProductVariantDTO> _updateValidator;

    public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<AddProductVariantDTO> addValidator,
        IValidator<UpdateProductVariantDTO> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<GetProductVariantDTO>> GetVariantsByProductIdAsync(Guid productId)
    {
        var spec = new ProductVariantsByProductSpecification(productId);
        var variants = await _unitOfWork.GetRepository<ProductVariant, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetProductVariantDTO>>(variants);
    }

    public async Task<GetProductVariantDTO> GetVariantByIdAsync(Guid id)
    {
        var variant = await _unitOfWork.GetRepository<ProductVariant, Guid>().GetByIdAsync(id);
        if (variant is null) throw new KeyNotFoundException($"Product variant with ID {id} not found.");
        return _mapper.Map<GetProductVariantDTO>(variant);
    }

    public async Task<GetProductVariantDTO> AddVariantAsync(AddProductVariantDTO dto)
    {
        var result = await _addValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var variant = _mapper.Map<ProductVariant>(dto);
        await _unitOfWork.GetRepository<ProductVariant, Guid>().AddAsync(variant);
        await _unitOfWork.SaveChangesAsync();

        if (dto.OptionValueIds.Count > 0)
        {
            foreach (var optionValueId in dto.OptionValueIds)
            {
                var pvov = new ProductVariantOptionValue
                {
                    ProductVariantId = variant.Id,
                    ProductOptionValueId = optionValueId
                };
                await _unitOfWork.GetRepository<ProductVariantOptionValue, Guid>().AddAsync(pvov);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        return _mapper.Map<GetProductVariantDTO>(variant);
    }

    public async Task<GetProductVariantDTO> UpdateVariantAsync(Guid id, UpdateProductVariantDTO dto)
    {
        
        var result = await _updateValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var spec = new ProductVariantSpecification(id);
        bool exists = await _unitOfWork.GetRepository<ProductVariant, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product variant with ID {id} not found.");
        
        var variant = _mapper.Map<ProductVariant>(dto);
        _unitOfWork.GetRepository<ProductVariant, Guid>().Update(variant);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetProductVariantDTO>(variant);
    }

    public async Task DeleteVariantAsync(Guid id)
    {
        var spec = new ProductVariantSpecification(id);
        bool exists = await _unitOfWork.GetRepository<ProductVariant, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Product variant with ID {id} not found.");
        var stub = new ProductVariant { Id = id };
        _unitOfWork.GetRepository<ProductVariant, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetProductVariantDTO> GetVariantBySKUAsync(Guid id, string sku)
    {
        var spec = new ProductVariantSpecification(id, sku);
        var variant = await _unitOfWork.GetRepository<ProductVariant, Guid>().GetFirstOrDefaultAsync(spec);
        if (variant is null) throw new KeyNotFoundException($"Product variant with SKU {sku} not found.");
        return _mapper.Map<GetProductVariantDTO>(variant);
    }

}
