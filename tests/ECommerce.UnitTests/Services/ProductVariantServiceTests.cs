using AutoMapper;
using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Product;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ProductVariantServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddProductVariantDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateProductVariantDTO>> _updateValidatorMock;
    private readonly IProductVariantService _productVariantService;

    public ProductVariantServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddProductVariantDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateProductVariantDTO>>();
        _productVariantService = new ProductVariantService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetVariantsByProductIdAsync_ShouldReturnMappedVariants()
    {
        var variants = new List<ProductVariant> { new() { Id = TestGuid.FromInt(1), Sku = "VAR-001" } };
        var variantDtos = new List<GetProductVariantDTO> { new() { Id = TestGuid.FromInt(1), Sku = "VAR-001" } };

        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.GetVariantsByProductIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(variants);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetProductVariantDTO>>(variants)).Returns(variantDtos);

        var result = await _productVariantService.GetVariantsByProductIdAsync(TestGuid.FromInt(1));

        result.Should().HaveCount(1);
        result.First().Sku.Should().Be("VAR-001");
    }

    [Fact]
    public async Task GetVariantByIdAsync_WhenExists_ShouldReturnVariant()
    {
        var variant = new ProductVariant { Id = TestGuid.FromInt(1), Sku = "VAR-001" };
        var variantDto = new GetProductVariantDTO { Id = TestGuid.FromInt(1), Sku = "VAR-001" };

        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.GetByIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);
        _mapperMock.Setup(m => m.Map<GetProductVariantDTO>(variant)).Returns(variantDto);

        var result = await _productVariantService.GetVariantByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.Sku.Should().Be("VAR-001");
    }

    [Fact]
    public async Task GetVariantByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductVariant?)null);

        var act = () => _productVariantService.GetVariantByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddVariantAsync_WithValidData_ShouldAddVariant()
    {
        var dto = new AddProductVariantDTO { Sku = "VAR-001", VariantName = "Large Red", ProductId = TestGuid.FromInt(1), OptionValueIds = new List<Guid>() };
        var variant = new ProductVariant { Sku = "VAR-001" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<ProductVariant>(dto)).Returns(variant);
        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.AddAsync(It.IsAny<ProductVariant>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productVariantService.AddVariantAsync(dto);

        _unitOfWorkMock.Verify(u => u.ProductVariantRepository.AddAsync(variant, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddVariantAsync_WithOptionValues_ShouldAddVariantAndOptions()
    {
        var dto = new AddProductVariantDTO
        {
            Sku = "VAR-001",
            VariantName = "Large Red",
            ProductId = TestGuid.FromInt(1),
            OptionValueIds = new List<Guid> { TestGuid.FromInt(1), TestGuid.FromInt(2) }
        };
        var variant = new ProductVariant { Id = TestGuid.FromInt(10), Sku = "VAR-001" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<ProductVariant>(dto)).Returns(variant);
        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.AddAsync(It.IsAny<ProductVariant>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.ProductVariantOptionValueRepository.AddAsync(It.IsAny<ProductVariantOptionValue>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productVariantService.AddVariantAsync(dto);

        _unitOfWorkMock.Verify(u => u.ProductVariantOptionValueRepository.AddAsync(
            It.IsAny<ProductVariantOptionValue>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddVariantAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddProductVariantDTO { Sku = "" };
        var failures = new List<ValidationFailure> { new("Sku", "SKU required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _productVariantService.AddVariantAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteVariantAsync_WhenExists_ShouldDeleteVariant()
    {
        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productVariantService.DeleteVariantAsync(TestGuid.FromInt(1));

        _unitOfWorkMock.Verify(u => u.ProductVariantRepository.DeleteAsync(It.IsAny<ProductVariant>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteVariantAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductVariantRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _productVariantService.DeleteVariantAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
