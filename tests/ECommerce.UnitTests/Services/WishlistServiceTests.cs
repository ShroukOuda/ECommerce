using AutoMapper;
using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Wishlist;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class WishlistServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddWishlistDTO>> _addValidatorMock;
    private readonly IWishlistService _wishlistService;

    public WishlistServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddWishlistDTO>>();
        _wishlistService = new WishlistService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object);
    }

    [Fact]
    public async Task GetWishlistByUserIdAsync_ShouldReturnMappedItems()
    {
        var items = new List<Wishlist> { new() { Id = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), UserId = "user1" } };
        var itemDtos = new List<GetWishlistDTO> { new() { Id = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), UserId = "user1" } };

        _unitOfWorkMock.Setup(u => u.WishlistRepository.GetWishlistByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetWishlistDTO>>(items)).Returns(itemDtos);

        var result = await _wishlistService.GetWishlistByUserIdAsync("user1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddToWishlistAsync_WithValidData_ShouldAddItem()
    {
        var dto = new AddWishlistDTO { ProductId = TestGuid.FromInt(1), UserId = "user1" };
        var wishlist = new Wishlist { ProductId = TestGuid.FromInt(1), UserId = "user1" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.WishlistRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Wishlist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Wishlist>(dto)).Returns(wishlist);
        _unitOfWorkMock.Setup(u => u.WishlistRepository.AddAsync(It.IsAny<Wishlist>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _wishlistService.AddToWishlistAsync(dto);

        _unitOfWorkMock.Verify(u => u.WishlistRepository.AddAsync(wishlist, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddToWishlistAsync_WhenAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var dto = new AddWishlistDTO { ProductId = TestGuid.FromInt(1), UserId = "user1" };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.WishlistRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Wishlist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _wishlistService.AddToWishlistAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddToWishlistAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddWishlistDTO { ProductId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("ProductId", "ProductId required") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _wishlistService.AddToWishlistAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RemoveFromWishlistAsync_WhenExists_ShouldRemoveItem()
    {
        var item = new Wishlist { Id = TestGuid.FromInt(1) };

        _unitOfWorkMock.Setup(u => u.WishlistRepository.GetByIdAsync(TestGuid.FromInt(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _wishlistService.RemoveFromWishlistAsync(TestGuid.FromInt(1));

        _unitOfWorkMock.Verify(u => u.WishlistRepository.DeleteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveFromWishlistAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.WishlistRepository.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wishlist?)null);

        var act = () => _wishlistService.RemoveFromWishlistAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
