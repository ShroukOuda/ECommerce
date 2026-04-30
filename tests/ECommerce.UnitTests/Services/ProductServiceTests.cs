using AutoMapper;
using ECommerce.Application.DTO.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Product;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileStorageService> _imageServiceMock;
    private readonly Mock<IValidator<AddProductDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateProductDTO>> _updateValidatorMock;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _imageServiceMock = new Mock<IFileStorageService>();
        _addValidatorMock = new Mock<IValidator<AddProductDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateProductDTO>>();
        _productService = new ProductService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _imageServiceMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnMappedProducts()
    {
        var products = new List<Product> { new() { Id = TestGuid.FromInt(1), Name = "Laptop" } };
        var productDtos = new List<GetProductDTO> { new() { Id = TestGuid.FromInt(1), Name = "Laptop" } };
        var productParams = new ProductParams();

        _unitOfWorkMock.Setup(u => u.ProductRepository.GetAllAsync(productParams, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products.AsEnumerable(), 1));
        _mapperMock.Setup(m => m.Map<IEnumerable<GetProductDTO>>(products))
            .Returns(productDtos);

        var result = await _productService.GetAllProductsAsync(productParams);

        result.Products.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenNoProducts_ShouldThrowKeyNotFoundException()
    {
        var productParams = new ProductParams();

        _unitOfWorkMock.Setup(u => u.ProductRepository.GetAllAsync(productParams, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IEnumerable<Product>)null!, 0));

        var act = () => _productService.GetAllProductsAsync(productParams);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        var product = new Product { Id = TestGuid.FromInt(1), Name = "Laptop" };
        var productDto = new GetProductDTO { Id = TestGuid.FromInt(1), Name = "Laptop" };

        _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<GetProductDTO>(product))
            .Returns(productDto);

        var result = await _productService.GetProductByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _productService.GetProductByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetProductByIdAsync_WithZeroId_ShouldThrowArgumentException()
    {
        var act = () => _productService.GetProductByIdAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddProductAsync_WithValidData_ShouldAddProduct()
    {
        var dto = new AddProductDTO { Name = "Laptop", Price = 999, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var product = new Product { Name = "Laptop" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
        _unitOfWorkMock.Setup(u => u.ProductRepository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productService.AddProductAsync(dto);

        _unitOfWorkMock.Verify(u => u.ProductRepository.AddAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddProductAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddProductDTO { Name = "" };
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _productService.AddProductAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductExists_ShouldDeleteProduct()
    {
        _unitOfWorkMock.Setup(u => u.ProductRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _imageServiceMock.Setup(s => s.DeleteFolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productService.DeleteProductAsync(TestGuid.FromInt(1));

        _unitOfWorkMock.Verify(u => u.ProductRepository.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _productService.DeleteProductAsync(TestGuid.FromInt(1));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteProductAsync_WithZeroId_ShouldThrowArgumentException()
    {
        var act = () => _productService.DeleteProductAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCount()
    {
        _unitOfWorkMock.Setup(u => u.ProductRepository.CountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var result = await _productService.GetTotalCountAsync();

        result.Should().Be(42);
    }
}
