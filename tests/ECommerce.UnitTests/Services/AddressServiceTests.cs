using AutoMapper;
using ECommerce.Application.DTO.Address;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Addresses;
using ECommerce.Domain.Entities.Users;
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
        var addresses = new List<Address> { new() { Id = TestGuid.FromInt(1), City = "Cairo", UserId = "user1" } };
        var addressDtos = new List<GetAddressDTO> { new() { Id = TestGuid.FromInt(1), City = "Cairo", UserId = "user1" } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetAllAsync(It.IsAny<AddressesByUserSpecification>()))
            .ReturnsAsync(addresses);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetAddressDTO>>(addresses)).Returns(addressDtos);

        var result = await _addressService.GetAddressesByUserIdAsync("user1");

        result.Should().HaveCount(1);
        result.First().City.Should().Be("Cairo");
    }

    [Fact]
    public async Task GetAddressByIdAsync_WhenExists_ShouldReturnAddress()
    {
        var address = new Address { Id = TestGuid.FromInt(1), City = "Cairo", UserId = "user1" };
        var addressDto = new GetAddressDTO { Id = TestGuid.FromInt(1), City = "Cairo", UserId = "user1" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(address);
        _mapperMock.Setup(m => m.Map<GetAddressDTO>(address)).Returns(addressDto);

        var result = await _addressService.GetAddressByIdAsync(TestGuid.FromInt(1), "user1");

        result.Should().NotBeNull();
        result.City.Should().Be("Cairo");
    }

    [Fact]
    public async Task GetAddressByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetByIdAsync(TestGuid.FromInt(999)))
            .ReturnsAsync((Address?)null);

        var act = () => _addressService.GetAddressByIdAsync(TestGuid.FromInt(999), "user1");

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
        var address = new Address { City = "Cairo", UserId = "user1" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Address>(dto)).Returns(address);
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().AddAsync(It.IsAny<Address>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _addressService.AddAddressAsync(dto, "user1");

        _unitOfWorkMock.Verify(u => u.GetRepository<Address, Guid>().AddAsync(address), Times.Once);
    }

    [Fact]
    public async Task AddAddressAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddAddressDTO { AddressLine1 = "" };
        var failures = new List<ValidationFailure> { new("AddressLine1", "Required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _addressService.AddAddressAsync(dto, "user1");

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateAddressAsync_WithValidData_ShouldUpdateAddress()
    {
        var dto = new UpdateAddressDTO
        {
            AddressLine1 = "456 New St",
            City = "Cairo",
            State = "Cairo",
            PostalCode = "11511",
            Country = "Egypt",
            Type = "Billing"
        };
        var address = new Address { Id = TestGuid.FromInt(1), City = "Cairo", UserId = "user1" };

        _updateValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetFirstOrDefaultAsync(It.IsAny<AddressSpecification>()))
            .ReturnsAsync(address);
        _mapperMock.Setup(m => m.Map<Address>(dto)).Returns(address);
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().Update(It.IsAny<Address>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _addressService.UpdateAddressAsync(TestGuid.FromInt(1), dto, "user1");

        _unitOfWorkMock.Verify(u => u.GetRepository<Address, Guid>().Update(address), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_WhenExists_ShouldDeleteAddress()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(new Address { Id = TestGuid.FromInt(1), UserId = "user1" });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _addressService.DeleteAddressAsync(TestGuid.FromInt(1), "user1");

        _unitOfWorkMock.Verify(u => u.GetRepository<Address, Guid>().Delete(It.IsAny<Address>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().ExistsAsync(It.IsAny<AddressSpecification>()))
            .ReturnsAsync(false);

        var act = () => _addressService.DeleteAddressAsync(TestGuid.FromInt(999), "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Theory]
    [InlineData("read")][InlineData("update")][InlineData("delete")]
    public async Task AnotherOwnersAddress_IsNeverAccessible(string operation)
    {
        var address = new Address { Id = TestGuid.FromInt(1), UserId = "victim" };
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetByIdAsync(address.Id)).ReturnsAsync(address);
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().GetFirstOrDefaultAsync(It.IsAny<AddressSpecification>())).ReturnsAsync(address);
        var dto = new UpdateAddressDTO();
        _updateValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        Func<Task> action = operation switch
        {
            "read" => async () => { await _addressService.GetAddressByIdAsync(address.Id, "attacker"); },
            "update" => async () => { await _addressService.UpdateAddressAsync(address.Id, dto, "attacker"); },
            _ => () => _addressService.DeleteAddressAsync(address.Id, "attacker")
        };
        await action.Should().ThrowAsync<KeyNotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAddress_ClientCannotAssignAnotherOwner()
    {
        var dto = new AddAddressDTO { UserId = "victim" };
        var address = new Address { UserId = "victim" };
        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Address>(dto)).Returns(address);
        _unitOfWorkMock.Setup(u => u.GetRepository<Address, Guid>().AddAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);
        await _addressService.AddAddressAsync(dto, "caller");
        address.UserId.Should().Be("caller");
        dto.UserId.Should().Be("caller");
    }
}
