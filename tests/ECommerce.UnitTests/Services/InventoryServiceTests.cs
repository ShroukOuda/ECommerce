using AutoMapper;
using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Inventories;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Inventory;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateInventoryHistoryDTO>> _createValidatorMock;
    private readonly IInventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateInventoryHistoryDTO>>();
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>().GetFirstOrDefaultAsync(It.IsAny<Application.Specifications.Products.ProductVariantSpecification>()))
            .ReturnsAsync(new ProductVariant { Id = TestGuid.FromInt(2), ProductId = TestGuid.FromInt(1), StockQuantity = 5 });
        _inventoryService = new InventoryService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    [Fact]
    public async Task GetHistoryByProductIdAsync_WhenProductExists_ShouldReturnMappedHistory()
    {
        var history = new List<InventoryHistory> { new() { Id = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1) } };
        var historyDtos = new List<GetInventoryHistoryDTO> { new() { Id = TestGuid.FromInt(1) } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().ExistsAsync(It.IsAny<Application.Specifications.Products.ProductSpecification>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.GetRepository<InventoryHistory, Guid>().GetAllAsync(It.IsAny<Application.Specifications.Inventories.InventoryHistoryByProductSpecification>()))
            .ReturnsAsync(history);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetInventoryHistoryDTO>>(history)).Returns(historyDtos);

        var result = await _inventoryService.GetHistoryByProductIdAsync(TestGuid.FromInt(1));

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetHistoryByProductIdAsync_WhenProductMissing_ShouldThrowNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().ExistsAsync(It.IsAny<Application.Specifications.Products.ProductSpecification>()))
            .ReturnsAsync(false);

        var act = () => _inventoryService.GetHistoryByProductIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WithValidData_ShouldUpdateStockAndAddHistory()
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = TestGuid.FromInt(2), QuantityChange = 10, ChangeType = InventoryChangeType.Restock };
        var product = new Product { Id = TestGuid.FromInt(1), StockQuantity = 5 };
        var history = new InventoryHistory();

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<InventoryHistory>(dto)).Returns(history);
        _unitOfWorkMock.Setup(u => u.GetRepository<InventoryHistory, Guid>().AddAsync(It.IsAny<InventoryHistory>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _inventoryService.AddInventoryHistoryAsync(TestGuid.FromInt(1), "user1", dto);

        product.StockQuantity.Should().Be(5, "legacy product stock must not be allocated implicitly");
        history.ProductVariantId.Should().Be(TestGuid.FromInt(2));
        history.ProductId.Should().Be(TestGuid.FromInt(1));
        history.UserId.Should().Be("user1");
        history.NewQuantity.Should().Be(15);
        _unitOfWorkMock.Verify(u => u.GetRepository<InventoryHistory, Guid>().AddAsync(history), Times.Once);
        _unitOfWorkMock.Verify(u => u.GetRepository<ProductVariant, Guid>().Update(It.Is<ProductVariant>(v => v.Id == TestGuid.FromInt(2) && v.StockQuantity == 15)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WhenChangeWouldMakeStockNegative_ShouldThrowInvalidOperationException()
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = TestGuid.FromInt(2), QuantityChange = -10, ChangeType = InventoryChangeType.Purchase };
        var product = new Product { Id = TestGuid.FromInt(1), StockQuantity = 5 };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(product);

        var act = () => _inventoryService.AddInventoryHistoryAsync(TestGuid.FromInt(1), "user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WhenProductMissing_ShouldThrowNotFoundException()
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = TestGuid.FromInt(2), QuantityChange = 10, ChangeType = InventoryChangeType.Restock };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(TestGuid.FromInt(999)))
            .ReturnsAsync((Product?)null);

        var act = () => _inventoryService.AddInventoryHistoryAsync(TestGuid.FromInt(999), "user1", dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateInventoryHistoryDTO { ChangeType = (InventoryChangeType)0 };
        var failures = new List<ValidationFailure> { new("ChangeType", "ChangeType required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _inventoryService.AddInventoryHistoryAsync(TestGuid.FromInt(1), "user1", dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task MissingVariant_RejectsWithoutMutation()
    {
        var dto = new CreateInventoryHistoryDTO { QuantityChange = 1, ChangeType = InventoryChangeType.Restock };
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(TestGuid.FromInt(1))).ReturnsAsync(new Product());
        await FluentActions.Invoking(() => _inventoryService.AddInventoryHistoryAsync(TestGuid.FromInt(1), "user1", dto)).Should().ThrowAsync<BadRequestException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task WrongProductsVariant_RejectsWithoutMutation()
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = TestGuid.FromInt(2), QuantityChange = 1, ChangeType = InventoryChangeType.Restock };
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(TestGuid.FromInt(3))).ReturnsAsync(new Product { Id = TestGuid.FromInt(3) });
        await FluentActions.Invoking(() => _inventoryService.AddInventoryHistoryAsync(TestGuid.FromInt(3), "user1", dto)).Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
