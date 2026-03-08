using AutoMapper;
using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Inventory;
using ECommerce.Core.Interfaces.Repositories;
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
        _inventoryService = new InventoryService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    [Fact]
    public async Task GetHistoryByProductIdAsync_ShouldReturnMappedHistory()
    {
        var history = new List<InventoryHistory> { new() { Id = 1, ProductId = 1 } };
        var historyDtos = new List<GetInventoryHistoryDTO> { new() { Id = 1, ProductId = 1 } };

        _unitOfWorkMock.Setup(u => u.InventoryHistoryRepository.GetHistoryByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetInventoryHistoryDTO>>(history)).Returns(historyDtos);

        var result = await _inventoryService.GetHistoryByProductIdAsync(1);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WithValidData_ShouldAddHistory()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = 1, NewQuantity = 100, ChangeType = "Restock" };
        var history = new InventoryHistory { ProductId = 1 };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<InventoryHistory>(dto)).Returns(history);
        _unitOfWorkMock.Setup(u => u.InventoryHistoryRepository.AddAsync(It.IsAny<InventoryHistory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _inventoryService.AddInventoryHistoryAsync(dto);

        _unitOfWorkMock.Verify(u => u.InventoryHistoryRepository.AddAsync(history, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = 0 };
        var failures = new List<ValidationFailure> { new("ProductId", "ProductId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _inventoryService.AddInventoryHistoryAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
