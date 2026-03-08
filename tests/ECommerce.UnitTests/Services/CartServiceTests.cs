using AutoMapper;
using ECommerce.Application.DTO.Cart;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Cart;
using ECommerce.Core.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddCartItemDTO>> _addItemValidatorMock;
    private readonly ICartService _cartService;

    public CartServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addItemValidatorMock = new Mock<IValidator<AddCartItemDTO>>();
        _cartService = new CartService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addItemValidatorMock.Object);
    }

    [Fact]
    public async Task GetCartByIdAsync_WhenCartExists_ShouldReturnCart()
    {
        var cart = new Cart { Id = 1, UserId = "user1" };
        var cartDto = new GetCartDTO { Id = 1, UserId = "user1" };

        _unitOfWorkMock.Setup(u => u.CartRepository.GetCartWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        _mapperMock.Setup(m => m.Map<GetCartDTO>(cart)).Returns(cartDto);

        var result = await _cartService.GetCartByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetCartByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.CartRepository.GetCartWithItemsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var act = () => _cartService.GetCartByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetActiveCartByUserIdAsync_WhenCartExists_ShouldReturnCart()
    {
        var cart = new Cart { Id = 1, UserId = "user1" };
        var cartDto = new GetCartDTO { Id = 1, UserId = "user1" };

        _unitOfWorkMock.Setup(u => u.CartRepository.GetActiveCartByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        _mapperMock.Setup(m => m.Map<GetCartDTO>(cart)).Returns(cartDto);

        var result = await _cartService.GetActiveCartByUserIdAsync("user1");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetActiveCartByUserIdAsync_WhenNoCart_ShouldReturnNull()
    {
        _unitOfWorkMock.Setup(u => u.CartRepository.GetActiveCartByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var result = await _cartService.GetActiveCartByUserIdAsync("user1");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCartItemAsync_WithValidData_ShouldAddItem()
    {
        var dto = new AddCartItemDTO { CartId = 1, ProductId = 1, Quantity = 2 };
        var cartItem = new CartItem { CartId = 1, ProductId = 1, Quantity = 2 };

        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<CartItem>(dto)).Returns(cartItem);
        _unitOfWorkMock.Setup(u => u.CartItemRepository.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _cartService.AddCartItemAsync(dto);

        _unitOfWorkMock.Verify(u => u.CartItemRepository.AddAsync(cartItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCartItemAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddCartItemDTO { CartId = 0 };
        var failures = new List<ValidationFailure> { new("CartId", "CartId required") };

        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _cartService.AddCartItemAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateCartItemAsync_WhenItemExists_ShouldUpdate()
    {
        var dto = new UpdateCartItemDTO { Id = 1, Quantity = 5 };
        var item = new CartItem { Id = 1, Quantity = 2 };

        _unitOfWorkMock.Setup(u => u.CartItemRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _cartService.UpdateCartItemAsync(dto);

        item.Quantity.Should().Be(5);
        _unitOfWorkMock.Verify(u => u.CartItemRepository.UpdateAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCartItemAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var dto = new UpdateCartItemDTO { Id = 999, Quantity = 5 };

        _unitOfWorkMock.Setup(u => u.CartItemRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CartItem?)null);

        var act = () => _cartService.UpdateCartItemAsync(dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RemoveCartItemAsync_WhenItemExists_ShouldRemove()
    {
        var item = new CartItem { Id = 1 };

        _unitOfWorkMock.Setup(u => u.CartItemRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _cartService.RemoveCartItemAsync(1);

        _unitOfWorkMock.Verify(u => u.CartItemRepository.DeleteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveCartItemAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.CartItemRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CartItem?)null);

        var act = () => _cartService.RemoveCartItemAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ClearCartAsync_WhenItemsExist_ShouldDeleteAll()
    {
        var items = new List<CartItem> { new() { Id = 1 }, new() { Id = 2 } };

        _unitOfWorkMock.Setup(u => u.CartItemRepository.GetItemsByCartIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _cartService.ClearCartAsync(1);

        _unitOfWorkMock.Verify(u => u.CartItemRepository.DeleteRangeAsync(items, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClearCartAsync_WhenNoItems_ShouldNotCallDelete()
    {
        _unitOfWorkMock.Setup(u => u.CartItemRepository.GetItemsByCartIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CartItem>());

        await _cartService.ClearCartAsync(1);

        _unitOfWorkMock.Verify(u => u.CartItemRepository.DeleteRangeAsync(It.IsAny<IEnumerable<CartItem>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
