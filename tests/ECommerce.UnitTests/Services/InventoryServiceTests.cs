using AutoMapper;
using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Inventories;
using ECommerce.Domain.Entities.Inventories;
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
        _inventoryService = new InventoryService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    [Fact]
    public async Task GetHistoryByProductIdAsync_ShouldReturnMappedHistory()
    {
        var history = new List<InventoryHistory> { new() { Id = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1) } };
        var historyDtos = new List<GetInventoryHistoryDTO> { new() { Id = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1) } };

        _unitOfWorkMock.Setup(u => u.GetRepository<InventoryHistory, Guid>().GetAllAsync(new InventoryHistoryByProductSpecification(TestGuid.FromInt(1))))
            .ReturnsAsync(history);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetInventoryHistoryDTO>>(history)).Returns(historyDtos);

        var result = await _inventoryService.GetHistoryByProductIdAsync(TestGuid.FromInt(1));

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WithValidData_ShouldAddHistory()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = TestGuid.FromInt(1), NewQuantity = 100, ChangeType = "Restock" };
        var history = new InventoryHistory { ProductId = TestGuid.FromInt(1) };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<InventoryHistory>(dto)).Returns(history);
        _unitOfWorkMock.Setup(u => u.GetRepository<InventoryHistory, Guid>().AddAsync(It.IsAny<InventoryHistory>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _inventoryService.AddInventoryHistoryAsync(dto);

        _unitOfWorkMock.Verify(u => u.GetRepository<InventoryHistory, Guid>().AddAsync(history), Times.Once);
    }

    [Fact]
    public async Task AddInventoryHistoryAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("ProductId", "ProductId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _inventoryService.AddInventoryHistoryAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
