using AutoMapper;
using ECommerce.Application.DTO.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Products;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IValidator<AddProductDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateProductDTO>> _updateValidatorMock;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _addValidatorMock = new Mock<IValidator<AddProductDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateProductDTO>>();
        _productService = new ProductService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _fileStorageServiceMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnMappedProducts()
    {
        var products = new List<Product> { new() { Id = TestGuid.FromInt(1), Name = "Laptop" } };
        var productDtos = new List<GetProductsDTO> { new() { Id = TestGuid.FromInt(1), Name = "Laptop" } };
        var productSpecParams = new ProductSpecParams();

        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<ProductSpecification>()))
            .ReturnsAsync(products);
        repositoryMock.Setup(r => r.CountAsync(It.IsAny<ProductCountSpecification>(), CancellationToken.None))
            .ReturnsAsync(1);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<GetProductsDTO>>(products))
            .Returns(productDtos.AsReadOnly());

        var result = await _productService.GetAllProductsAsync(productSpecParams);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenNoProducts_ShouldThrowKeyNotFoundException()
    {
        var productParams = new ProductSpecParams();
        
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<ProductSpecification>()))
            .ReturnsAsync((List<Product>)null!);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var act = () => _productService.GetAllProductsAsync(productParams);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        var product = new Product { Id = TestGuid.FromInt(1), Name = "Laptop" };
        var productDto = new GetProductDetailsDTO { Id = TestGuid.FromInt(1), Name = "Laptop" };

        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ProductDetailsSpecification>()))
            .ReturnsAsync(product);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetProductDetailsDTO>(product))
            .Returns(productDto);

        var result = await _productService.GetProductByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ProductDetailsSpecification>()))
            .ReturnsAsync((Product?)null);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var act = () => _productService.GetProductByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithZeroId_ShouldThrowKeyNotFoundException()
    {
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ProductDetailsSpecification>()))
            .ReturnsAsync((Product?)null);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var act = () => _productService.GetProductByIdAsync(Guid.Empty);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetSimilarProductsAsync_WhenProductExists_ShouldReturnSimilarProducts()
    {
        var product = new Product { Id = TestGuid.FromInt(1), Name = "Laptop" };
        var similarProducts = new List<Product> { new() { Id = TestGuid.FromInt(2), Name = "Gaming Laptop" } };
        var similarProductDtos = new List<GetProductsDTO> { new() { Id = TestGuid.FromInt(2), Name = "Gaming Laptop" } };
        var paginationParams = new PaginationParams();

        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ProductDetailsSpecification>()))
            .ReturnsAsync(product);
        repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<SimilarProductsSpecification>()))
            .ReturnsAsync(similarProducts);
        repositoryMock.Setup(r => r.CountAsync(It.IsAny<SimilarProductsCountSpecification>(), CancellationToken.None))
            .ReturnsAsync(1);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<GetProductsDTO>>(similarProducts))
            .Returns(similarProductDtos.AsReadOnly());

        var result = await _productService.GetSimilarProductsAsync(TestGuid.FromInt(1), paginationParams);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Gaming Laptop");
    }

    [Fact]
    public async Task GetSimilarProductsAsync_WhenProductNotFound_ShouldThrowKeyNotFoundException()
    {
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ProductDetailsSpecification>()))
            .ReturnsAsync((Product?)null);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var act = () => _productService.GetSimilarProductsAsync(TestGuid.FromInt(999), new PaginationParams());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddProductAsync_WithValidData_ShouldAddProduct()
    {
        var dto = new AddProductDTO
        {
            Name = "New Laptop"
        };

        var product = new Product
        {
            Id = TestGuid.FromInt(1),
            Name = dto.Name
        };

        _addValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(m => m.Map<Product>(dto))
            .Returns(product);

        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        _unitOfWorkMock
            .Setup(u => u.GetRepository<Product, Guid>())
            .Returns(repositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _productService.AddProductAsync(dto);

        repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddProductAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddProductDTO
        {
            Name = "" // Invalid name
        };

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };

        _addValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _productService.AddProductAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ShouldUpdateProduct()
    {
        var dto = new UpdateProductDTO
        {
            Id = TestGuid.FromInt(1),
            Name = "Updated Laptop"
        };

        var product = new Product
        {
            Id = dto.Id,
            Name = dto.Name
        };

        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        repositoryMock
            .Setup(r => r.ExistsAsync(
                It.IsAny<ProductSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(m => m.Map<Product>(dto))
            .Returns(product);

        _unitOfWorkMock
            .Setup(u => u.GetRepository<Product, Guid>())
            .Returns(repositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _productService.UpdateProductAsync(dto);

        repositoryMock.Verify(
            r => r.Update(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    [Fact]
    public async Task UpdateProductAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new UpdateProductDTO
        {
            Id = TestGuid.FromInt(1),
            Name = "" // Invalid name
        };

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _productService.UpdateProductAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductNotFound_ShouldThrowKeyNotFoundException()
    {
        var dto = new UpdateProductDTO
        {
            Id = TestGuid.FromInt(999),
            Name = "Non-existent Product"
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock
            .Setup(r => r.ExistsAsync(
                It.IsAny<ProductSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.GetRepository<Product, Guid>())
            .Returns(repositoryMock.Object);

        var act = () => _productService.UpdateProductAsync(dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }  

    [Fact]
    public async Task DeleteProductAsync_WhenProductExists_ShouldDeleteProduct()
    {
        var productId = TestGuid.FromInt(1);
        
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<ProductSpecification>(), CancellationToken.None))
            .ReturnsAsync(true);
        repositoryMock.Setup(r => r.Delete(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);
        _fileStorageServiceMock.Setup(s => s.DeleteFolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productService.DeleteProductAsync(productId);

        repositoryMock.Verify(u => u.Delete(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<ProductSpecification>(), CancellationToken.None))
            .ReturnsAsync(false);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var act = () => _productService.DeleteProductAsync(TestGuid.FromInt(1));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteProductAsync_WithZeroId_ShouldThrowKeyNotFoundException()
    {
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<ProductSpecification>(), CancellationToken.None))
            .ReturnsAsync(false);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var act = () => _productService.DeleteProductAsync(Guid.Empty);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCount()
    {
        var repositoryMock = new Mock<IGenericRepository<Product, Guid>>();
        repositoryMock.Setup(r => r.CountAsync(It.IsAny<ProductCountSpecification>(), CancellationToken.None))
            .ReturnsAsync(42);
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()).Returns(repositoryMock.Object);

        var result = await _productService.GetTotalCountAsync();

        result.Should().Be(42);
    }
}
