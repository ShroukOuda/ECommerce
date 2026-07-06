using AutoMapper;
using ECommerce.Application.DTO.Brand;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Brands;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class BrandServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddBrandDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateBrandDTO>> _updateValidatorMock;
    private readonly IBrandService _brandService;

    public BrandServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddBrandDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateBrandDTO>>();
        _brandService = new BrandService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAllBrandsAsync_ShouldReturnMappedBrands()
    {
        // Arrange
        var brands = new List<Brand> { new() { Id = TestGuid.FromInt(1), Name = "Nike" } };
        var brandDtos = new List<GetBrandDTO> { new() { Id = TestGuid.FromInt(1), Name = "Nike" } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Brand, Guid>().GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(brands);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetBrandDTO>>(brands))
            .Returns(brandDtos);

        // Act
        var result = await _brandService.GetAllBrandsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Nike");
    }

    [Fact]
    public async Task GetBrandByIdAsync_WhenBrandExists_ShouldReturnBrand()
    {
        // Arrange
        var brand = new Brand { Id = TestGuid.FromInt(1), Name = "Nike" };
        var brandDto = new GetBrandDTO { Id = TestGuid.FromInt(1), Name = "Nike" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Brand, Guid>().GetByIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);
        _mapperMock.Setup(m => m.Map<GetBrandDTO>(brand))
            .Returns(brandDto);

        // Act
        var result = await _brandService.GetBrandByIdAsync(TestGuid.FromInt(1));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Nike");
    }

    [Fact]
    public async Task GetBrandByIdAsync_WhenBrandNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.GetRepository<Brand, Guid>().GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        // Act
        var act = () => _brandService.GetBrandByIdAsync(TestGuid.FromInt(999));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddBrandAsync_WithValidData_ShouldAddBrand()
    {
        // Arrange
        var dto = new AddBrandDTO { Name = "Adidas" };
        var brand = new Brand { Name = "Adidas" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Brand>(dto))
            .Returns(brand);
        _unitOfWorkMock.Setup(u => u.GetRepository<Brand, Guid>().AddAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _brandService.AddBrandAsync(dto);

        // Assert
        _unitOfWorkMock.Verify(u => u.GetRepository<Brand, Guid>().AddAsync(brand, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddBrandAsync_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new AddBrandDTO { Name = "" };
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var act = () => _brandService.AddBrandAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteBrandAsync_WhenBrandExists_ShouldDeleteBrand()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.GetRepository<Brand, Guid>().ExistsAsync(
            new BrandSpecification(TestGuid.FromInt(1)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _brandService.DeleteBrandAsync(TestGuid.FromInt(1));

        // Assert
        _unitOfWorkMock.Verify(u => u.GetRepository<Brand, Guid>().Delete(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBrandAsync_WhenBrandNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.GetRepository<Brand, Guid>().ExistsAsync(
            new BrandSpecification(TestGuid.FromInt(1)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var act = () => _brandService.DeleteBrandAsync(TestGuid.FromInt(999));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
