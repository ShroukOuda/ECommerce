using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Core.Entities.Product;
using ECommerce.Core.Interfaces.Repositories;

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

    public async Task<IEnumerable<GetProductVariantDTO>> GetVariantsByProductIdAsync(int productId, CancellationToken ct = default)
    {
        var variants = await _unitOfWork.ProductVariantRepository.GetVariantsByProductIdAsync(productId, ct);
        return _mapper.Map<IEnumerable<GetProductVariantDTO>>(variants);
    }

    public async Task<GetProductVariantDTO> GetVariantByIdAsync(int id, CancellationToken ct = default)
    {
        var variant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(id, ct);
        if (variant is null) throw new KeyNotFoundException($"Product variant with ID {id} not found.");
        return _mapper.Map<GetProductVariantDTO>(variant);
    }

    public async Task AddVariantAsync(AddProductVariantDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var variant = _mapper.Map<ProductVariant>(dto);
        await _unitOfWork.ProductVariantRepository.AddAsync(variant, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        if (dto.OptionValueIds.Count > 0)
        {
            foreach (var optionValueId in dto.OptionValueIds)
            {
                var pvov = new ProductVariantOptionValue
                {
                    ProductVariantId = variant.Id,
                    ProductOptionValueId = optionValueId
                };
                await _unitOfWork.ProductVariantOptionValueRepository.AddAsync(pvov, ct);
            }
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateVariantAsync(UpdateProductVariantDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var variant = _mapper.Map<ProductVariant>(dto);
        await _unitOfWork.ProductVariantRepository.UpdateAsync(variant, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteVariantAsync(int id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.ProductVariantRepository.ExistsAsync(v => v.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Product variant with ID {id} not found.");
        var stub = new ProductVariant { Id = id };
        await _unitOfWork.ProductVariantRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
