using AutoMapper;
using ECommerce.Application.DTO.Shipping;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Shipping;
using ECommerce.Core.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ShippingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateShippingDTO>> _createValidatorMock;
    private readonly IShippingService _shippingService;

    public ShippingServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateShippingDTO>>();
        _shippingService = new ShippingService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    [Fact]
    public async Task GetShippingsByOrderIdAsync_ShouldReturnMappedShippings()
    {
        var shippings = new List<Shipping> { new() { Id = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1) } };
        var shippingDtos = new List<GetShippingDTO> { new() { Id = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1) } };

        _unitOfWorkMock.Setup(u => u.ShippingRepository.GetShippingsByOrderIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shippings);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetShippingDTO>>(shippings)).Returns(shippingDtos);

        var result = await _shippingService.GetShippingsByOrderIdAsync(TestGuid.FromInt(1));

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetShippingByIdAsync_WhenExists_ShouldReturnShipping()
    {
        var shipping = new Shipping { Id = TestGuid.FromInt(1), TrackingNumber = "SHP-001" };
        var shippingDto = new GetShippingDTO { Id = TestGuid.FromInt(1), TrackingNumber = "SHP-001" };

        _unitOfWorkMock.Setup(u => u.ShippingRepository.GetByIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shipping);
        _mapperMock.Setup(m => m.Map<GetShippingDTO>(shipping)).Returns(shippingDto);

        var result = await _shippingService.GetShippingByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.TrackingNumber.Should().Be("SHP-001");
    }

    [Fact]
    public async Task GetShippingByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ShippingRepository.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shipping?)null);

        var act = () => _shippingService.GetShippingByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateShippingAsync_WithValidData_ShouldCreateShipping()
    {
        var dto = new CreateShippingDTO { OrderId = TestGuid.FromInt(1), AddressId = TestGuid.FromInt(1), Method = "Standard", Cost = 9.99m };
        var shippingDto = new GetShippingDTO { Id = TestGuid.FromInt(1) };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.ShippingRepository.AddAsync(It.IsAny<Shipping>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetShippingDTO>(It.IsAny<Shipping>())).Returns(shippingDto);

        var result = await _shippingService.CreateShippingAsync(dto);

        result.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.ShippingRepository.AddAsync(It.IsAny<Shipping>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateShippingAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateShippingDTO { OrderId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("OrderId", "OrderId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _shippingService.CreateShippingAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
