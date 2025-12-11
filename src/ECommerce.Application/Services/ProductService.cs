namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageManagementService _imageManagementService;
    private readonly IValidator<AddProductDTO> _addProductDtoValidator;
    private readonly IValidator<UpdateProductDTO> _updateProductDtoValidator;
    private readonly IValidator<UploadProductPhotoDto> _uploadProductPhotoDtoValidator;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        IImageManagementService imageManagementService,
        IValidator<AddProductDTO> addProductDtoValidator,
        IValidator<UpdateProductDTO> updateProductDtoValidator,
        IValidator<UploadProductPhotoDto> uploadProductPhotoDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageManagementService = imageManagementService;
        _addProductDtoValidator = addProductDtoValidator;
        _updateProductDtoValidator = updateProductDtoValidator;
        _uploadProductPhotoDtoValidator = uploadProductPhotoDtoValidator;
    }
    
    
    public async Task<IEnumerable<GetProductDTO>> GetAllProductsAsync(ProductParams productParams)
    {
        var products = await _unitOfWork.ProductRepository.GetAllProductsAsync(productParams);
        if (products is null)
            throw new KeyNotFoundException("Products not found");
        
        return  _mapper.Map<IEnumerable<GetProductDTO>>(products);
    }

    public async Task<GetProductDTO> GetProductByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than zero.", nameof(id));
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p=>p.Category, p=>p.Photos);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        return _mapper.Map<GetProductDTO>(product);
    }

    public async Task AddProductAsync(AddProductDTO productDTO)
    {
        var validationResult = await _addProductDtoValidator.ValidateAsync(productDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        Product? product = null; 
        try
        {
            product = _mapper.Map<Product>(productDTO);
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
        }
        catch (Exception e)
        { 
            throw new Exception($"Error Adding Product: {e.Message}", e);
        }
       
    }

    public async Task AddPhotoAsync(int ProductId, UploadProductPhotoDto productPhotoDTO)
    {
        var validationResult = await _uploadProductPhotoDtoValidator.ValidateAsync(productPhotoDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {ProductId} not found.");

        try
        {
            var folder = $"Products/{ProductId}";
            var imagePaths = await _imageManagementService.AddImageAsync(productPhotoDTO.Photos, folder);
            var photos = imagePaths.Select(path => new Photo
            {
                ImageName = path,
                ProductId = ProductId,
            }).ToList();
            await _unitOfWork.PhotoRepository.AddRangeAsync(photos);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Error adding photos to product: {e.Message}", e);
        }
    }

    public async Task UpdateProductAsync(UpdateProductDTO productDTO)
    {
        var validationResult = await _updateProductDtoValidator.ValidateAsync(productDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        try
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productDTO.Id, p => p.Photos);
            
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {productDTO.Id} not found.");

            _mapper.Map(productDTO, existingProduct);
            await _unitOfWork.ProductRepository.UpdateAsync(existingProduct);
            await _unitOfWork.SaveChangesAsync();
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating product: {e.Message}", e);
        }
       
    }
    public async Task DeleteProductAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than zero.", nameof(id));
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        
        var folder = $"Products/{product.Id}";
        _imageManagementService.DeleteImagesFolder(folder);
        await _unitOfWork.ProductRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task DeletePhotoAsync(int photoId)
    {
        var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with ID {photoId} not found");

        _imageManagementService.DeleteImageFile(photo.ImageName);
        await _unitOfWork.PhotoRepository.DeleteAsync(photoId);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<int> GetTotalCountAsync()
    {
        int totalCount = await _unitOfWork.ProductRepository.CountAsync();
        return totalCount;
    }
    
}