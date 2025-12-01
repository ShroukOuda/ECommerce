using AutoMapper;
using E_Commerece.Application.Interfaces;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Interfaces;
using E_Commerece.Core.Services;

namespace E_Commerece.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageManagementService _imageManagementService;

    public ProductService(IUnitOfWork unitOfWork,IMapper mapper, IImageManagementService imageManagementService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageManagementService = imageManagementService;
    }

    public async Task<IEnumerable<GetProductDTO>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync(p=>p.Category, p=>p.Photos);
        if (products is null)
            throw new KeyNotFoundException("Products not found");
        return _mapper.Map<IEnumerable<GetProductDTO>>(products);
    }

    public async Task<GetProductDTO> GetProductByIdAsync(int id)
    {
        ValidateId(id);
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p=>p.Category, p=>p.Photos);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        return _mapper.Map<GetProductDTO>(product);
    }

    public async Task AddProductAsync(AddProductDTO productDTO)
    {
        ValidateProductDTO(productDTO);
        Product? product = null; 
        try
        {
            product = _mapper.Map<Product>(productDTO);
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            if (productDTO.Photos != null && productDTO.Photos.Any())
            {
                var imagePaths = await _imageManagementService.AddImageAsync(productDTO.Photos, product.Id.ToString());
                var photos = imagePaths.Select(path => new Photo
                {
                    ImageName = path,
                    ProductId = product.Id,
                }).ToList();
                await _unitOfWork.PhotoRepository.AddRangeAsync(photos);
                await _unitOfWork.SaveChangesAsync();
            }

            
        }
        catch (Exception e)
        { 
            if (product != null && product.Id > 0)
            {
                _imageManagementService.DeleteImagesFolder(product.Id.ToString());
            }
            throw new Exception($"Error Adding Product: {e.Message}", e);
        }
       
    }

    public async Task UpdateProductAsync(UpdateProductDTO productDTO)
    {
        ValidateUpdateProductDTO(productDTO);

        try
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productDTO.Id, p => p.Photos);
            
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {productDTO.Id} not found.");

            _mapper.Map(productDTO, existingProduct);
            await _unitOfWork.ProductRepository.UpdateAsync(existingProduct);
            await _unitOfWork.SaveChangesAsync();
            
            if (productDTO.PhotosToDelete != null && productDTO.PhotosToDelete.Any())
            {

                var photosToDelete = existingProduct.Photos
                    .Where(p => productDTO.PhotosToDelete.Contains(p.Id))
                    .ToList();

                foreach (var photo in photosToDelete)
                {
                    _imageManagementService.DeleteImageFile(photo.ImageName);
                }

                await _unitOfWork.PhotoRepository.DeleteRangeAsync(photosToDelete);
                
            }

            if (productDTO.NewPhotos != null && productDTO.NewPhotos.Any())
            {
                var folder = existingProduct.Id.ToString(); 
                var newImagePaths = await _imageManagementService.AddImageAsync(productDTO.NewPhotos, folder);
                var newPhotos = newImagePaths.Select(path => new Photo
                {
                    ImageName = path,
                    ProductId = existingProduct.Id,
                }).ToList();

                await _unitOfWork.PhotoRepository.AddRangeAsync(newPhotos);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating product: {e.Message}", e);
        }
       
    }
    public async Task DeleteProductAsync(int id)
    {
        ValidateId(id);
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        
        _imageManagementService.DeleteImagesFolder(product.Id.ToString());
        await _unitOfWork.ProductRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
    
    private void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than zero.", nameof(id));
    }

    private void ValidateProductDTO(AddProductDTO productDTO)
    {
        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO), "Product data cannot be null.");

        if (string.IsNullOrWhiteSpace(productDTO.Name))
            throw new ArgumentException("Product name is required.", nameof(productDTO.Name));

        if (productDTO.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero.", nameof(productDTO.Price));

        if (productDTO.CategoryId <= 0)
            throw new ArgumentException("Valid category ID is required.", nameof(productDTO.CategoryId));
    }

    private void ValidateUpdateProductDTO(UpdateProductDTO productDTO)
    {
        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO), "Product data cannot be null.");

        if (productDTO.Id <= 0)
            throw new ArgumentException("Product ID must be greater than zero.", nameof(productDTO.Id));

        if (string.IsNullOrWhiteSpace(productDTO.Name))
            throw new ArgumentException("Product name is required.", nameof(productDTO.Name));

        if (productDTO.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero.", nameof(productDTO.Price));
        

        if (productDTO.CategoryId <= 0)
            throw new ArgumentException("Valid category ID is required.", nameof(productDTO.CategoryId));
    }
}