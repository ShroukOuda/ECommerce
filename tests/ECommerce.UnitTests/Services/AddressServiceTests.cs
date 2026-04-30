using AutoMapper;
using ECommerce.Application.DTO.Address;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.User;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class AddressServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddAddressDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateAddressDTO>> _updateValidatorMock;
    private readonly IAddressService _addressService;

    public AddressServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddAddressDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateAddressDTO>>();
        _addressService = new AddressService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAddressesByUserIdAsync_ShouldReturnMappedAddresses()
    {
        var addresses = new List<Address> { new() { Id = TestGuid.FromInt(1), City = "Cairo" } };
        var addressDtos = new List<GetAddressDTO> { new() { Id = TestGuid.FromInt(1), City = "Cairo" } };

        _unitOfWorkMock.Setup(u => u.AddressRepository.GetAddressesByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(addresses);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetAddressDTO>>(addresses)).Returns(addressDtos);

        var result = await _addressService.GetAddressesByUserIdAsync("user1");

        result.Should().HaveCount(1);
        result.First().City.Should().Be("Cairo");
    }

    [Fact]
    public async Task GetAddressByIdAsync_WhenExists_ShouldReturnAddress()
    {
        var address = new Address { Id = TestGuid.FromInt(1), City = "Cairo" };
        var addressDto = new GetAddressDTO { Id = TestGuid.FromInt(1), City = "Cairo" };

        _unitOfWorkMock.Setup(u => u.AddressRepository.GetByIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);
        _mapperMock.Setup(m => m.Map<GetAddressDTO>(address)).Returns(addressDto);

        var result = await _addressService.GetAddressByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.City.Should().Be("Cairo");
    }

    [Fact]
    public async Task GetAddressByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.AddressRepository.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address?)null);

        var act = () => _addressService.GetAddressByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddAddressAsync_WithValidData_ShouldAddAddress()
    {
        var dto = new AddAddressDTO
        {
            UserId = "user1",
            AddressLine1 = "123 Main St",
            City = "Cairo",
            State = "Cairo",
            PostalCode = "11511",
            Country = "Egypt",
            Type = "Shipping"
        };
        var address = new Address { City = "Cairo" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Address>(dto)).Returns(address);
        _unitOfWorkMock.Setup(u => u.AddressRepository.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _addressService.AddAddressAsync(dto);

        _unitOfWorkMock.Verify(u => u.AddressRepository.AddAsync(address, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAddressAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddAddressDTO { AddressLine1 = "" };
        var failures = new List<ValidationFailure> { new("AddressLine1", "Required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _addressService.AddAddressAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateAddressAsync_WithValidData_ShouldUpdateAddress()
    {
        var dto = new UpdateAddressDTO
        {
            Id = TestGuid.FromInt(1),
            AddressLine1 = "456 New St",
            City = "Cairo",
            State = "Cairo",
            PostalCode = "11511",
            Country = "Egypt",
            Type = "Billing"
        };
        var address = new Address { Id = TestGuid.FromInt(1), City = "Cairo" };

        _updateValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Address>(dto)).Returns(address);
        _unitOfWorkMock.Setup(u => u.AddressRepository.UpdateAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _addressService.UpdateAddressAsync(dto);

        _unitOfWorkMock.Verify(u => u.AddressRepository.UpdateAsync(address, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_WhenExists_ShouldDeleteAddress()
    {
        _unitOfWorkMock.Setup(u => u.AddressRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Address, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _addressService.DeleteAddressAsync(TestGuid.FromInt(1));

        _unitOfWorkMock.Verify(u => u.AddressRepository.DeleteAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.AddressRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Address, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _addressService.DeleteAddressAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
