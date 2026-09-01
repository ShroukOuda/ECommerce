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
        Guid productId,
        CancellationToken ct = default)
    {
        var productSpec = new ProductSpecification(productId);

        var exists = await _unitOfWork
            .GetRepository<Product, Guid>()
            .ExistsAsync(productSpec, ct);

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
        CreateInventoryHistoryDTO dto,
        CancellationToken ct = default)
    {
        
        var result = await _createValidator
            .ValidateAsync(dto, ct);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

   
        var productRepository =
            _unitOfWork.GetRepository<Product, Guid>();


        var product = await productRepository
            .GetByIdAsync(productId, ct);

        if (product is null)
            throw new NotFoundException(
                $"Product with ID {productId} was not found.");

        var newQuantity =
            product.StockQuantity + dto.QuantityChange;

        if (newQuantity < 0)
            throw new InvalidOperationException(
                "Inventory quantity cannot be negative.");

        product.StockQuantity = newQuantity;

        var history = _mapper.Map<InventoryHistory>(dto);

        history.ProductId = productId;
        history.UserId = userId;
        history.NewQuantity = newQuantity;


        await _unitOfWork
            .GetRepository<InventoryHistory, Guid>()
            .AddAsync(history, ct);

        productRepository.Update(product, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<GetInventoryHistoryDTO>(history);
    }
}