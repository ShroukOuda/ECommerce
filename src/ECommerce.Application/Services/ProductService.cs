using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Products;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IValidator<AddProductDTO> _addProductDtoValidator;
    private readonly IValidator<UpdateProductDTO> _updateProductDtoValidator;


    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        IFileStorageService fileStorageService,
        IValidator<AddProductDTO> addProductDtoValidator,
        IValidator<UpdateProductDTO> updateProductDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _addProductDtoValidator = addProductDtoValidator;
        _updateProductDtoValidator = updateProductDtoValidator;
    }
    
    
    public async Task<PaginatedResult<GetProductDTO>> GetAllProductsAsync(
        ProductSpecParams productSpecParams,
        CancellationToken ct = default)
    {
        var productSpec = new ProductSpecification(productSpecParams);
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(productSpec);
        if (products is null)
            throw new KeyNotFoundException("Products not found");
        
        var productCountSpec = new ProductCountSpecification(productSpecParams);
        var totalItems = await _unitOfWork.GetRepository<Product, Guid>().CountAsync(productCountSpec);
        var mapProducts = _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
        return new PaginatedResult<GetProductDTO>(mapProducts, totalItems, productSpecParams.PageNumber, productSpecParams.PageSize);
    }

    public async Task<GetProductDTO> GetProductByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.GetRepository<Product, Guid>().GetByIdAsync(id, ct);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        return _mapper.Map<GetProductDTO>(product);
    }

    public async Task<IReadOnlyList<GetProductDTO>> GetFeaturedProductsAsync()
    {
        var spec = new FeaturedProductsSpecification();
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
    }
    public async Task<IReadOnlyList<GetProductDTO>> GetBestSellerProductsAsync()
    {
        var spec = new BestSellerProductsSpecification();
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
    }
    public async Task<IReadOnlyList<GetProductDTO>> GetNewArrivalProductsAsync()
    {
        var spec = new NewArrivalProductsSpecification();
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
    }
    public async Task<IReadOnlyList<GetProductDTO>> GetHotDealProductsAsync()
    {
        var spec = new HotDealProductsSpecification();
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
    }
    public async Task<IReadOnlyList<GetProductDTO>> GetTopRatedProductsAsync()
    {
        var spec = new TopRatedProductsSpecification();
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
    }

    public async Task<IReadOnlyList<GetProductDTO>> GetLowStockProductsAsync()
    {
        var spec = new LowStockProductsSpecification();
        var products = await _unitOfWork.GetRepository<Product, Guid>().GetAllAsync(spec);

        return _mapper.Map<IReadOnlyList<GetProductDTO>>(products);
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
            await _unitOfWork.GetRepository<Product, Guid>().AddAsync(product);
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
            var spec = new ProductSpecification(productDto.Id);
            bool exists = await _unitOfWork.GetRepository<Product, Guid>().ExistsAsync(spec);
            
            if (!exists)
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");

            var product = _mapper.Map<Product>(productDto);
            _unitOfWork.GetRepository<Product, Guid>().Update(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating product: {e.Message}", e);
        }
       
    }
    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new ProductSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Product, Guid>().ExistsAsync(spec);
        if (!exists)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        var folderPath = $"products/{id}";
        await _fileStorageService.DeleteFolderAsync(folderPath, ct);
        Product productStub = new Product { Id = id };
        _unitOfWork.GetRepository<Product, Guid>().Delete(productStub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    
    }
    
    public async Task<int> GetTotalCountAsync()
    {
        var spec = new ProductCountSpecification();
        int totalCount = await _unitOfWork.GetRepository<Product, Guid>().CountAsync(spec);
        return totalCount;
    }
    
}