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
        var ptroducts = await _unitOfWork.ProductRepository.GetAllAsync(p=>p.Category, p=>p.Photos);
        return _mapper.Map<IEnumerable<GetProductDTO>>(ptroducts);
    }

    public async Task<GetProductDTO> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p=>p.Category, p=>p.Photos);
        return _mapper.Map<GetProductDTO>(product);
    }

    public async Task AddProductAsync(AddProductDTO productDTO)
    {
        var product = _mapper.Map<Product>(productDTO);
        await _unitOfWork.ProductRepository.AddAsync(product);
        var imagePaths = await _imageManagementService.AddImageAsync(productDTO.Photos, productDTO.Name);
        var photos = imagePaths.Select(path => new Photo
        {
            ImageName = path,
            ProductId = product.Id,
        }).ToList();
        
        await _unitOfWork.PhotoRepository.AddRangeAsync(photos);
    }

    public async Task UpdateProductAsync(UpdateProductDTO productDTO)
    {
        var product = _mapper.Map<Product>(productDTO);
        await _unitOfWork.ProductRepository.UpdateAsync(product);
    }
    public async Task DeleteProductAsync(int id)
    {
        await _unitOfWork.ProductRepository.DeleteAsync(id);
    }
}