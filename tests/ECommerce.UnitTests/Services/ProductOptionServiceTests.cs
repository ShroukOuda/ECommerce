using AutoMapper;
using ECommerce.Application.DTO.ProductOption;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Product;
using ECommerce.Core.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ProductOptionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddProductOptionDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateProductOptionDTO>> _updateValidatorMock;
    private readonly Mock<IValidator<AddProductOptionValueDTO>> _addValueValidatorMock;
    private readonly IProductOptionService _productOptionService;

    public ProductOptionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddProductOptionDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateProductOptionDTO>>();
        _addValueValidatorMock = new Mock<IValidator<AddProductOptionValueDTO>>();
        _productOptionService = new ProductOptionService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object,
            _addValueValidatorMock.Object);
    }

    [Fact]
    public async Task GetOptionsByProductIdAsync_ShouldReturnMappedOptions()
    {
        var options = new List<ProductOption> { new() { Id = 1, Name = "Color" } };
        var optionDtos = new List<GetProductOptionDTO> { new() { Id = 1, Name = "Color" } };

        _unitOfWorkMock.Setup(u => u.ProductOptionRepository.GetOptionsByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(options);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetProductOptionDTO>>(options)).Returns(optionDtos);

        var result = await _productOptionService.GetOptionsByProductIdAsync(1);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Color");
    }

    [Fact]
    public async Task GetOptionByIdAsync_WhenExists_ShouldReturnOption()
    {
        var option = new ProductOption { Id = 1, Name = "Size" };
        var optionDto = new GetProductOptionDTO { Id = 1, Name = "Size" };

        _unitOfWorkMock.Setup(u => u.ProductOptionRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(option);
        _mapperMock.Setup(m => m.Map<GetProductOptionDTO>(option)).Returns(optionDto);

        var result = await _productOptionService.GetOptionByIdAsync(1);

        result.Should().NotBeNull();
        result.Name.Should().Be("Size");
    }

    [Fact]
    public async Task GetOptionByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductOptionRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductOption?)null);

        var act = () => _productOptionService.GetOptionByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddOptionAsync_WithValidData_ShouldAddOption()
    {
        var dto = new AddProductOptionDTO { Name = "Color", DisplayType = "Dropdown", Type = "VariantSelector", AttributeKey = "color", ProductId = 1 };
        var option = new ProductOption { Name = "Color" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<ProductOption>(dto)).Returns(option);
        _unitOfWorkMock.Setup(u => u.ProductOptionRepository.AddAsync(It.IsAny<ProductOption>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productOptionService.AddOptionAsync(dto);

        _unitOfWorkMock.Verify(u => u.ProductOptionRepository.AddAsync(option, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddOptionAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddProductOptionDTO { Name = "" };
        var failures = new List<ValidationFailure> { new("Name", "Name required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _productOptionService.AddOptionAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteOptionAsync_WhenExists_ShouldDeleteOption()
    {
        _unitOfWorkMock.Setup(u => u.ProductOptionRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductOption, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productOptionService.DeleteOptionAsync(1);

        _unitOfWorkMock.Verify(u => u.ProductOptionRepository.DeleteAsync(It.IsAny<ProductOption>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOptionAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductOptionRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductOption, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _productOptionService.DeleteOptionAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddOptionValueAsync_WithValidData_ShouldAddValue()
    {
        var dto = new AddProductOptionValueDTO { Value = "Red", Label = "Red", OptionId = 1 };
        var value = new ProductOptionValue { Value = "Red" };

        _addValueValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<ProductOptionValue>(dto)).Returns(value);
        _unitOfWorkMock.Setup(u => u.ProductOptionValueRepository.AddAsync(It.IsAny<ProductOptionValue>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productOptionService.AddOptionValueAsync(dto);

        _unitOfWorkMock.Verify(u => u.ProductOptionValueRepository.AddAsync(value, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOptionValueAsync_WhenExists_ShouldDeleteValue()
    {
        _unitOfWorkMock.Setup(u => u.ProductOptionValueRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductOptionValue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _productOptionService.DeleteOptionValueAsync(1);

        _unitOfWorkMock.Verify(u => u.ProductOptionValueRepository.DeleteAsync(It.IsAny<ProductOptionValue>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOptionValueAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductOptionValueRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductOptionValue, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _productOptionService.DeleteOptionValueAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
