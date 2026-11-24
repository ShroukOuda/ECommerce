using AutoMapper;
using ECommerce.Application.DTO.Cart;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Application.Specifications.Products;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Product;
using ECommerce.Domain.Interfaces.Repositories;
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
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync((CartDetailsSpecification _) => new Cart { Id = Guid.NewGuid(), UserId = "user1", ExpiresAt = DateTime.UtcNow.AddDays(1) });
        _cartService = new CartService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addItemValidatorMock.Object);
    }

    [Fact]
    public async Task GetCartByIdAsync_WhenCartExists_ShouldReturnCart()
    {
        var cart = new Cart { Id = TestGuid.FromInt(1), UserId = "user1", ExpiresAt = DateTime.UtcNow.AddDays(1) };
        var cartDto = new GetCartDTO { Id = TestGuid.FromInt(1), UserId = "user1", ExpiresAt = DateTime.UtcNow.AddDays(1) };

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _mapperMock.Setup(m => m.Map<GetCartDTO>(cart)).Returns(cartDto);

        var result = await _cartService.GetCartByIdAsync(TestGuid.FromInt(1), "user1");

        result.Should().NotBeNull();
        result.Id.Should().Be(TestGuid.FromInt(1));
    }

    [Fact]
    public async Task GetCartByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync((Cart?)null);

        var act = () => _cartService.GetCartByIdAsync(TestGuid.FromInt(999), "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetActiveCartByUserIdAsync_WhenCartExists_ShouldReturnCart()
    {
        var cart = new Cart { Id = TestGuid.FromInt(1), UserId = "user1", ExpiresAt = DateTime.UtcNow.AddDays(1) };
        var cartDto = new GetCartDTO { Id = TestGuid.FromInt(1), UserId = "user1", ExpiresAt = DateTime.UtcNow.AddDays(1) };

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(It.IsAny<CartByUserSpecification>()))
            .ReturnsAsync(cart);
        _mapperMock.Setup(m => m.Map<GetCartDTO>(cart)).Returns(cartDto);

        var result = await _cartService.GetActiveCartByUserIdAsync("user1");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetActiveCartByUserIdAsync_WhenNoCart_ShouldReturnNull()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(It.IsAny<CartByUserSpecification>()))
            .ReturnsAsync((Cart?)null);

        var result = await _cartService.GetActiveCartByUserIdAsync("user1");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCartItemAsync_WithValidData_ShouldAddItemWithCatalogPrice()
    {
        var dto = new AddCartItemDTO { CartId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), VariantId = TestGuid.FromInt(2), Quantity = 2 };
        var cartItem = new CartItem { CartId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), Quantity = 2 };
        var product = new Product
        {
            Id = TestGuid.FromInt(1),
            Status = ProductStatus.Published,
            BasePrice = 100m,
            SalePrice = 80m // on sale
        };
        var variant = new ProductVariant
        {
            Id = TestGuid.FromInt(2),
            ProductId = TestGuid.FromInt(1),
            Status = ProductVariantStatus.Active,
            PriceAdjustment = 10m
        };

        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<CartItem>(dto)).Returns(cartItem);
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(dto.ProductId))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);
        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().AddAsync(It.IsAny<CartItem>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _cartService.AddCartItemAsync(dto, "user1");

        // 80 sale price + 10 variant adjustment, server-side
        cartItem.Price.Should().Be(90m);
        _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().AddAsync(cartItem), Times.Once);
    }

    [Fact]
    public async Task AddCartItemAsync_WhenProductNotPublished_ShouldThrowArgumentException()
    {
        var dto = new AddCartItemDTO { CartId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), VariantId = TestGuid.FromInt(2), Quantity = 1 };
        var product = new Product { Id = TestGuid.FromInt(1), Status = ProductStatus.Draft };

        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(dto.ProductId))
            .ReturnsAsync(product);

        var act = () => _cartService.AddCartItemAsync(dto, "user1");

        await act.Should().ThrowAsync<ArgumentException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddCartItemAsync_WhenVariantNotForProduct_ShouldThrowArgumentException()
    {
        var dto = new AddCartItemDTO { CartId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), VariantId = TestGuid.FromInt(2), Quantity = 1 };
        var product = new Product { Id = TestGuid.FromInt(1), Status = ProductStatus.Published };
        var variant = new ProductVariant { Id = TestGuid.FromInt(2), ProductId = TestGuid.FromInt(999), Status = ProductVariantStatus.Active };

        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(dto.ProductId))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);

        var act = () => _cartService.AddCartItemAsync(dto, "user1");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCartItemAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddCartItemDTO { CartId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("CartId", "CartId required") };

        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _cartService.AddCartItemAsync(dto, "user1");

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateCartItemAsync_WhenItemExists_ShouldUpdate()
    {
        var dto = new UpdateCartItemDTO { Quantity = 5 };
        var item = new CartItem { Id = TestGuid.FromInt(1), Quantity = 2 };

        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(item);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _cartService.UpdateCartItemAsync(TestGuid.FromInt(1), dto, "user1");

        item.Quantity.Should().Be(5);
        _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().Update(item), Times.Once);
    }

    [Fact]
    public async Task UpdateCartItemAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var dto = new UpdateCartItemDTO { Quantity = 5 };

        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().GetByIdAsync(TestGuid.FromInt(999)))
            .ReturnsAsync((CartItem?)null);

        var act = () => _cartService.UpdateCartItemAsync(TestGuid.FromInt(999), dto, "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RemoveCartItemAsync_WhenItemExists_ShouldRemove()
    {
        var item = new CartItem { Id = TestGuid.FromInt(1) };

        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().ExistsAsync(It.IsAny<CartItemSpecification>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _cartService.RemoveCartItemAsync(TestGuid.FromInt(1), TestGuid.FromInt(1), "user1");

        _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().Delete(
            It.Is<CartItem>(c => c.Id == TestGuid.FromInt(1))), Times.Once);
    }

    [Fact]
    public async Task RemoveCartItemAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().ExistsAsync(It.IsAny<CartItemSpecification>()))
            .ReturnsAsync(false);

        var act = () => _cartService.RemoveCartItemAsync(TestGuid.FromInt(1), TestGuid.FromInt(999), "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ClearCartAsync_WhenItemsExist_ShouldDeleteAll()
    {
        var items = new List<CartItem> { new() { Id = TestGuid.FromInt(1) }, new() { Id = TestGuid.FromInt(2) } };

        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().GetAllAsync(It.IsAny<CartItemsByCartSpecification>()))
            .ReturnsAsync(items);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _cartService.ClearCartAsync(TestGuid.FromInt(1), "user1");

        _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().DeleteRange(items), Times.Once);
    }

    [Fact]
    public async Task ClearCartAsync_WhenNoItems_ShouldNotCallDelete()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().GetAllAsync(It.IsAny<CartItemsByCartSpecification>()))
            .ReturnsAsync(new List<CartItem>());

        await _cartService.ClearCartAsync(TestGuid.FromInt(1), "user1");

        _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().DeleteRange(It.IsAny<IEnumerable<CartItem>>()), Times.Never);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AddWithoutVariant_UsesProductOnlyWhenNoVariantsExist(bool hasVariants)
    {
        var dto = new AddCartItemDTO { CartId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1 };
        _addItemValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(dto.ProductId)).ReturnsAsync(new Product { Id = dto.ProductId, BasePrice = 17m, Status = ProductStatus.Published });
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>().ExistsAsync(It.IsAny<ProductVariantsByProductSpecification>())).ReturnsAsync(hasVariants);
        var item = new CartItem();
        _mapperMock.Setup(m => m.Map<CartItem>(dto)).Returns(item);
        _unitOfWorkMock.Setup(u => u.GetRepository<CartItem, Guid>().AddAsync(item)).Returns(Task.CompletedTask);
        if (hasVariants)
            await FluentActions.Invoking(() => _cartService.AddCartItemAsync(dto, "user1")).Should().ThrowAsync<ECommerce.Domain.Exceptions.BadRequestException>();
        else
        {
            await _cartService.AddCartItemAsync(dto, "user1");
            item.Price.Should().Be(17m);
            _unitOfWorkMock.Verify(u => u.GetRepository<CartItem, Guid>().AddAsync(item), Times.Once);
        }
    }
}
