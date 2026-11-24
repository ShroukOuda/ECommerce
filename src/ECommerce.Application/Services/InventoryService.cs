using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Specifications.Inventories;
using ECommerce.Application.Specifications.Products;
using ECommerce.Domain.Entities.Inventories;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInventoryHistoryDTO> _createValidator;

    public InventoryService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateInventoryHistoryDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetInventoryHistoryDTO>> GetHistoryByProductIdAsync(
        Guid productId)
    {
        var productSpec = new ProductSpecification(productId);

        var exists = await _unitOfWork
            .GetRepository<Product, Guid>()
            .ExistsAsync(productSpec);

        if (!exists)
            throw new NotFoundException(
                $"Product with ID {productId} was not found.");

        var historySpec =
            new InventoryHistoryByProductSpecification(productId);

        var history = await _unitOfWork
            .GetRepository<InventoryHistory, Guid>()
            .GetAllAsync(historySpec);

        return _mapper.Map<IEnumerable<GetInventoryHistoryDTO>>(history);
    }

    public async Task<GetInventoryHistoryDTO> AddInventoryHistoryAsync(
        Guid productId,
        string userId,
        CreateInventoryHistoryDTO dto)
    {
        
        var result = await _createValidator
            .ValidateAsync(dto);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

   
        var productRepository =
            _unitOfWork.GetRepository<Product, Guid>();


        var product = await productRepository
            .GetByIdAsync(productId);

        if (product is null)
            throw new NotFoundException(
                $"Product with ID {productId} was not found.");

        var variantRepository = _unitOfWork.GetRepository<ProductVariant, Guid>();
        ProductVariant? variant = null;
        if (dto.ProductVariantId.HasValue)
        {
            variant = await variantRepository.GetFirstOrDefaultAsync(new ProductVariantSpecification(dto.ProductVariantId.Value));
            if (variant is null || variant.ProductId != productId)
                throw new NotFoundException("The selected variant does not belong to this product.");
        }
        else if (await variantRepository.ExistsAsync(new ProductVariantsByProductSpecification(productId)))
            throw new BadRequestException("Choose a productVariantId for a product with variants.");
        int newQuantity;
        try { newQuantity = checked((variant?.StockQuantity ?? product.StockQuantity) + dto.QuantityChange); }
        catch (OverflowException) { throw new BadRequestException("Inventory quantity is too large."); }
        if (newQuantity < 0) throw new BusinessRuleException("Inventory quantity cannot be negative.");
        var stockStatus = newQuantity == 0 ? ECommerce.Domain.Enums.Inventory.StockStatus.OutOfStock : ECommerce.Domain.Enums.Inventory.StockStatus.InStock;
        if (variant is not null) { variant.StockQuantity = newQuantity; variant.StockStatus = stockStatus; }
        else { product.StockQuantity = newQuantity; product.StockStatus = stockStatus; }

        var history = _mapper.Map<InventoryHistory>(dto);

        history.ProductId = productId;
        history.ProductVariantId = variant?.Id;
        history.UserId = userId;
        history.NewQuantity = newQuantity;


        await _unitOfWork
            .GetRepository<InventoryHistory, Guid>()
            .AddAsync(history);

        if (variant is not null) variantRepository.Update(variant);
        else productRepository.Update(product);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<GetInventoryHistoryDTO>(history);
    }
}
