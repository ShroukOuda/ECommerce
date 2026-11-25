using AutoMapper;
using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Coupons;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Interfaces.Repositories;
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
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly ICouponService _couponService;

    public CouponServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddCouponDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateCouponDTO>>();
        _cartServiceMock = new Mock<ICartService>();
        _couponService = new CouponService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _cartServiceMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAllCouponsAsync_ShouldReturnMappedCoupons()
    {
        var coupons = new List<Coupon> { new() { Id = TestGuid.FromInt(1), Code = "SAVE10" } };
        var couponDtos = new List<GetCouponDTO> { new() { Id = TestGuid.FromInt(1), Code = "SAVE10" } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().GetAllAsync())
            .ReturnsAsync(coupons);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetCouponDTO>>(coupons)).Returns(couponDtos);

        var result = await _couponService.GetAllCouponsAsync();

        result.Should().HaveCount(1);
        result.First().Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task GetCouponByIdAsync_WhenExists_ShouldReturnCoupon()
    {
        var coupon = new Coupon { Id = TestGuid.FromInt(1), Code = "SAVE10" };
        var couponDto = new GetCouponDetailsDTO { Id = TestGuid.FromInt(1), Code = "SAVE10" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(coupon);
        _mapperMock.Setup(m => m.Map<GetCouponDetailsDTO>(coupon)).Returns(couponDto);

        var result = await _couponService.GetCouponByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task GetCouponByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().GetByIdAsync(TestGuid.FromInt(999)))
            .ReturnsAsync((Coupon?)null);

        var act = () => _couponService.GetCouponByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetCouponByCodeAsync_WhenExists_ShouldReturnCoupon()
    {
        var coupon = new Coupon { Id = TestGuid.FromInt(1), Code = "SAVE10" };
        var couponDto = new GetCouponDTO { Id = TestGuid.FromInt(1), Code = "SAVE10" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().GetFirstOrDefaultAsync(It.IsAny<CouponByCodeSpecification>()))
            .ReturnsAsync(coupon);
        _mapperMock.Setup(m => m.Map<GetCouponDTO>(coupon)).Returns(couponDto);

        var result = await _couponService.GetCouponByCodeAsync("SAVE10");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCouponByCodeAsync_WhenNotFound_ShouldReturnNull()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().GetFirstOrDefaultAsync(It.IsAny<CouponByCodeSpecification>()))
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
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().AddAsync(It.IsAny<Coupon>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _couponService.AddCouponAsync(dto);

        _unitOfWorkMock.Verify(u => u.GetRepository<Coupon, Guid>().AddAsync(coupon), Times.Once);
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
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().ExistsAsync(
            It.IsAny<CouponSpecification>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _couponService.DeleteCouponAsync(TestGuid.FromInt(1));

        _unitOfWorkMock.Verify(u => u.GetRepository<Coupon, Guid>().Delete(It.IsAny<Coupon>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCouponAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().ExistsAsync(
            It.IsAny<CouponSpecification>()))
            .ReturnsAsync(false);

        var act = () => _couponService.DeleteCouponAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ValidateCouponForSubtotalAsync_UsesSuppliedSubtotalNotCartSnapshot()
    {
        var cartId = TestGuid.FromInt(3);
        var cart = new ECommerce.Domain.Entities.Carts.Cart
        {
            Id = cartId, UserId = "user1",
            CartItems = { new ECommerce.Domain.Entities.Carts.CartItem { Price = 10m, Quantity = 1 } } // snapshot 10, below min
        };
        var coupon = new Coupon
        {
            Id = TestGuid.FromInt(4), Code = "TEN", Status = ECommerce.Domain.Enums.Coupon.CouponStatus.Active,
            ValidFrom = DateTime.UtcNow.AddDays(-1), ValidUntil = DateTime.UtcNow.AddDays(1),
            MinPurchaseAmount = 100m,
            DiscountType = ECommerce.Domain.Enums.Coupon.DiscountType.Percentage, DiscountValue = 10m
        };
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>().GetFirstOrDefaultAsync(It.IsAny<CouponByCodeSpecification>())).ReturnsAsync(coupon);
        _unitOfWorkMock.Setup(u => u.GetRepository<ECommerce.Domain.Entities.Carts.Cart, Guid>()
            .GetFirstOrDefaultAsync(It.IsAny<ECommerce.Application.Specifications.Carts.CartDetailsSpecification>())).ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.GetRepository<CouponUsage, Guid>().CountAsync(It.IsAny<CouponUsageByUserSpecification>())).ReturnsAsync(0);

        var ok = await _couponService.ValidateCouponForSubtotalAsync("user1", cartId, "TEN", 200m);
        ok.DiscountAmount.Should().Be(20m); // on the supplied 200, though the cart snapshot is 10

        var act = () => _couponService.ValidateCouponForSubtotalAsync("user1", cartId, "TEN", 50m);
        await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.BusinessRuleException>(); // minimum not met on supplied subtotal
    }
}
