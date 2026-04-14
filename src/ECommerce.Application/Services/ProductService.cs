using ECommerce.Core.Entities.Product;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageManagementService _imageManagementService;
    private readonly IValidator<AddProductDTO> _addProductDtoValidator;
    private readonly IValidator<UpdateProductDTO> _updateProductDtoValidator;


    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        IImageManagementService imageManagementService,
        IValidator<AddProductDTO> addProductDtoValidator,
        IValidator<UpdateProductDTO> updateProductDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageManagementService = imageManagementService;
        _addProductDtoValidator = addProductDtoValidator;
        _updateProductDtoValidator = updateProductDtoValidator;
    }
    
    
    public async Task<(IEnumerable<GetProductDTO> Products, int TotalCount)> GetAllProductsAsync(
        ProductParams productParams,
        CancellationToken ct = default)
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync(productParams, ct);
        if (products.Products is null)
            throw new KeyNotFoundException("Products not found");
        
        var mapProducts = _mapper.Map<IEnumerable<GetProductDTO>>(products.Products);
        return (mapProducts, products.TotalCount);
    }

    public async Task<GetProductDTO> GetProductByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, ct);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        return _mapper.Map<GetProductDTO>(product);
    }

    public async Task AddProductAsync(AddProductDTO productDTO, CancellationToken cancellationToken = default)
    {
        var validationResult = await _addProductDtoValidator.ValidateAsync(productDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        Product? product = null; 
        try
        {
            product = _mapper.Map<Product>(productDTO);
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        }
        catch (Exception e)
        { 
            throw new Exception($"Error Adding Product: {e.Message}", e);
        }
       
    }
    
    public async Task UpdateProductAsync(UpdateProductDTO productDto, CancellationToken ct = default)
    {
        var validationResult = await _updateProductDtoValidator.ValidateAsync(productDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        try
        {
            bool exists = await _unitOfWork.ProductRepository.ExistsAsync(p => p.Id == productDto.Id, ct);
            
            if (!exists)
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");

            var product = _mapper.Map<Product>(productDto);
            await _unitOfWork.ProductRepository.UpdateAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating product: {e.Message}", e);
        }
       
    }
    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.ProductRepository.ExistsAsync(p => p.Id == id, ct);
        if (!exists)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        var folderPath = $"products/{id}";
        await _imageManagementService.DeleteFolderAsync(folderPath, ct);
        Product productStub = new Product { Id = id };
        await _unitOfWork.ProductRepository.DeleteAsync(productStub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    
    }
    
    public async Task<int> GetTotalCountAsync()
    {
        int totalCount = await _unitOfWork.ProductRepository.CountAsync();
        return totalCount;
    }
    
}