using AutoMapper;
using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Coupon;
using ECommerce.Core.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class CouponServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddCouponDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateCouponDTO>> _updateValidatorMock;
    private readonly ICouponService _couponService;

    public CouponServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddCouponDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateCouponDTO>>();
        _couponService = new CouponService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAllCouponsAsync_ShouldReturnMappedCoupons()
    {
        var coupons = new List<Coupon> { new() { Id = 1, Code = "SAVE10" } };
        var couponDtos = new List<GetCouponDTO> { new() { Id = 1, Code = "SAVE10" } };

        _unitOfWorkMock.Setup(u => u.CouponRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupons);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetCouponDTO>>(coupons)).Returns(couponDtos);

        var result = await _couponService.GetAllCouponsAsync();

        result.Should().HaveCount(1);
        result.First().Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task GetCouponByIdAsync_WhenExists_ShouldReturnCoupon()
    {
        var coupon = new Coupon { Id = 1, Code = "SAVE10" };
        var couponDto = new GetCouponDTO { Id = 1, Code = "SAVE10" };

        _unitOfWorkMock.Setup(u => u.CouponRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);
        _mapperMock.Setup(m => m.Map<GetCouponDTO>(coupon)).Returns(couponDto);

        var result = await _couponService.GetCouponByIdAsync(1);

        result.Should().NotBeNull();
        result.Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task GetCouponByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.CouponRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var act = () => _couponService.GetCouponByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetCouponByCodeAsync_WhenExists_ShouldReturnCoupon()
    {
        var coupon = new Coupon { Id = 1, Code = "SAVE10" };
        var couponDto = new GetCouponDTO { Id = 1, Code = "SAVE10" };

        _unitOfWorkMock.Setup(u => u.CouponRepository.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);
        _mapperMock.Setup(m => m.Map<GetCouponDTO>(coupon)).Returns(couponDto);

        var result = await _couponService.GetCouponByCodeAsync("SAVE10");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCouponByCodeAsync_WhenNotFound_ShouldReturnNull()
    {
        _unitOfWorkMock.Setup(u => u.CouponRepository.GetByCodeAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var result = await _couponService.GetCouponByCodeAsync("INVALID");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCouponAsync_WithValidData_ShouldAddCoupon()
    {
        var dto = new AddCouponDTO { Code = "SAVE10", DiscountValue = 10, UsageLimit = 100, ValidFrom = DateTime.UtcNow, ValidUntil = DateTime.UtcNow.AddDays(30) };
        var coupon = new Coupon { Code = "SAVE10" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Coupon>(dto)).Returns(coupon);
        _unitOfWorkMock.Setup(u => u.CouponRepository.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _couponService.AddCouponAsync(dto);

        _unitOfWorkMock.Verify(u => u.CouponRepository.AddAsync(coupon, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCouponAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddCouponDTO { Code = "" };
        var failures = new List<ValidationFailure> { new("Code", "Code required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _couponService.AddCouponAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteCouponAsync_WhenExists_ShouldDeleteCoupon()
    {
        _unitOfWorkMock.Setup(u => u.CouponRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Coupon, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _couponService.DeleteCouponAsync(1);

        _unitOfWorkMock.Verify(u => u.CouponRepository.DeleteAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCouponAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.CouponRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Coupon, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _couponService.DeleteCouponAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
