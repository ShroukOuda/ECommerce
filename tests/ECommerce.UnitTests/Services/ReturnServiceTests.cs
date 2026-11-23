using AutoMapper;
using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Coupons;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Application.Specifications.Returns;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Returns;
using ECommerce.Domain.Enums.Return;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ReturnServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateReturnRequestDTO>> _createValidatorMock;
    private readonly IReturnService _returnService;

    public ReturnServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateReturnRequestDTO>>();
        _returnService = new ReturnService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);

        // Default: no coupons on the order. Individual refund
        // tests override this setup.
        _unitOfWorkMock.Setup(u => u.GetRepository<CouponUsage, Guid>()
                .GetAllAsync(It.IsAny<CouponUsagesByOrderSpecification>()))
            .ReturnsAsync(new List<CouponUsage>());
    }

    private void SetupGetReturn(ReturnRequest? returnRequest)
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ReturnDetailsSpecification>()))
            .ReturnsAsync(returnRequest);
    }

    // ---------------- Customer ----------------

    [Fact]
    public async Task GetMyReturnsAsync_ShouldReturnMappedReturns()
    {
        var returns = new List<ReturnRequest> { new() { Id = TestGuid.FromInt(1), UserId = "user1" } };
        var returnDtos = new List<ReturnResponseDTO> { new() { Id = TestGuid.FromInt(1) } };

        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>()
                .GetAllAsync(It.IsAny<ReturnsByUserSpecification>()))
            .ReturnsAsync(returns);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<ReturnResponseDTO>>(returns))
            .Returns(returnDtos);

        var result = await _returnService.GetMyReturnsAsync("user1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMyReturnByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ReturnDetailsByUserSpecification>()))
            .ReturnsAsync((ReturnRequest?)null);

        var act = () => _returnService.GetMyReturnByIdAsync(TestGuid.FromInt(999), "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateReturnAsync_WithValidData_ShouldCreateRequestedReturn()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Broken" } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 3 }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        returnRequest.UserId.Should().Be("user1");
        returnRequest.Status.Should().Be(ReturnStatus.Requested);
        returnRequest.ReturnNumber.Should().NotBeNullOrEmpty();
        returnRequest.ReturnItems.Should().ContainSingle()
            .Which.ProductId.Should().Be(TestGuid.FromInt(7));
        returnRequest.ReturnHistories.Should().ContainSingle()
            .Which.NewStatus.Should().Be(ReturnStatus.Requested);
        _unitOfWorkMock.Verify(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(returnRequest), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateReturnAsync_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), Quantity = 1 } }
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync((Order?)null);

        var act = () => _returnService.CreateReturnAsync("user1", dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()), Times.Never);
    }

    [Fact]
    public async Task CreateReturnAsync_WhenOrderBelongsToAnotherUser_ShouldThrowKeyNotFoundException()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), Quantity = 1 } }
        };
        var order = new Order { Id = dto.OrderId, UserId = "someone-else" };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);

        var act = () => _returnService.CreateReturnAsync("user1", dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateReturnAsync_WhenItemNotInOrder_ShouldThrowArgumentException()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(99), Quantity = 1 } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(7), Quantity = 3 }
            }
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(new ReturnRequest { OrderId = dto.OrderId });

        var act = () => _returnService.CreateReturnAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReturnAsync_WhenQuantityExceedsReturnable_ShouldThrowArgumentException()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = orderItemId, Quantity = 2 } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 2 }
            }
        };
        var existingItems = new List<ReturnItem>
        {
            new() { OrderItemId = orderItemId, Quantity = 1 }
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(existingItems);
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(new ReturnRequest { OrderId = dto.OrderId });

        var act = () => _returnService.CreateReturnAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReturnAsync_WithoutCoupon_ShouldRefundFullProRataLineValue()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = orderItemId, Quantity = 2, Reason = "Broken" } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 3, UnitPrice = 50m, TotalPrice = 150m }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        // 150 line value / 3 units * 2 returned = 100
        returnRequest.ReturnItems.Should().ContainSingle()
            .Which.RefundAmount.Should().Be(100m);
        returnRequest.RefundAmount.Should().Be(100m);
    }

    [Fact]
    public async Task CreateReturnAsync_WithCoupon_ShouldAllocateDiscountProportionally()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Broken" } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 2, UnitPrice = 50m, TotalPrice = 100m },
                new() { Id = TestGuid.FromInt(2), ProductId = TestGuid.FromInt(8), Quantity = 1, UnitPrice = 100m, TotalPrice = 100m }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _unitOfWorkMock.Setup(u => u.GetRepository<CouponUsage, Guid>()
                .GetAllAsync(It.IsAny<CouponUsagesByOrderSpecification>()))
            .ReturnsAsync(new List<CouponUsage> { new() { OrderId = dto.OrderId, DiscountAmount = 40m } });
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        // Subtotal 200, coupon 40 -> line share 20 -> line net 80 -> unit net 40
        returnRequest.ReturnItems.Should().ContainSingle()
            .Which.RefundAmount.Should().Be(40m);
        returnRequest.RefundAmount.Should().Be(40m);
    }

    [Fact]
    public async Task CreateReturnAsync_WhenDiscountExceedsLineValue_ShouldClampRefundToZero()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Broken" } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 1, UnitPrice = 10m, TotalPrice = 10m }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _unitOfWorkMock.Setup(u => u.GetRepository<CouponUsage, Guid>()
                .GetAllAsync(It.IsAny<CouponUsagesByOrderSpecification>()))
            .ReturnsAsync(new List<CouponUsage> { new() { OrderId = dto.OrderId, DiscountAmount = 50m } });
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        returnRequest.ReturnItems.Should().ContainSingle()
            .Which.RefundAmount.Should().Be(0m);
        returnRequest.RefundAmount.Should().Be(0m);
    }

    [Fact]
    public async Task CreateReturnAsync_WithMultipleItems_ShouldSumItemRefunds()
    {
        var firstItemId = TestGuid.FromInt(1);
        var secondItemId = TestGuid.FromInt(2);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO>
            {
                new() { OrderItemId = firstItemId, Quantity = 1, Reason = "Broken" },
                new() { OrderItemId = secondItemId, Quantity = 1, Reason = "Wrong size" }
            }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = firstItemId, ProductId = TestGuid.FromInt(7), Quantity = 1, UnitPrice = 100m, TotalPrice = 100m },
                new() { Id = secondItemId, ProductId = TestGuid.FromInt(8), Quantity = 2, UnitPrice = 25m, TotalPrice = 50m }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        // 100 + (50/2 * 1) = 125
        returnRequest.RefundAmount.Should().Be(125m);
    }

    [Fact]
    public async Task CreateReturnAsync_WithDuplicateOrderItemIdsExceedingQuantity_ShouldThrowArgumentException()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO>
            {
                new() { OrderItemId = orderItemId, Quantity = 2, Reason = "Broken" },
                new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Broken again" }
            }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 2 }
            }
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(new ReturnRequest { OrderId = dto.OrderId });

        // Each entry alone fits the remaining quantity (2), but
        // combined they exceed it.
        var act = () => _returnService.CreateReturnAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()), Times.Never);
    }

    [Fact]
    public async Task CreateReturnAsync_RepeatedPartialRefunds_ShouldNotExceedLineNetValue()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Broken" } }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 2, UnitPrice = 0.025m, TotalPrice = 0.05m }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>
            {
                // A previous return already refunded 0.03 (the
                // rounded half of the 0.05 line).
                new() { OrderItemId = orderItemId, Quantity = 1, RefundAmount = 0.03m }
            });
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        // Rounded per-request refund would be 0.03 again; capped
        // at the remaining 0.02 of the 0.05 line.
        returnRequest.ReturnItems.Should().ContainSingle()
            .Which.RefundAmount.Should().Be(0.02m);
        returnRequest.RefundAmount.Should().Be(0.02m);
    }

    [Fact]
    public async Task CreateReturnAsync_DuplicateEntriesInOneRequest_ShouldCapCombinedRefundAtLineNetValue()
    {
        var orderItemId = TestGuid.FromInt(1);
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO>
            {
                new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Broken" },
                new() { OrderItemId = orderItemId, Quantity = 1, Reason = "Also broken" }
            }
        };
        var order = new Order
        {
            Id = dto.OrderId,
            UserId = "user1",
            OrderItems = new List<OrderItem>
            {
                new() { Id = orderItemId, ProductId = TestGuid.FromInt(7), Quantity = 2, UnitPrice = 0.025m, TotalPrice = 0.05m }
            }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        // 0.03 + capped 0.02 = the full 0.05 line value, no more.
        returnRequest.ReturnItems.Should().HaveCount(2);
        returnRequest.RefundAmount.Should().Be(0.05m);
    }

    [Fact]
    public async Task CreateReturnAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateReturnRequestDTO { OrderId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("OrderId", "OrderId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _returnService.CreateReturnAsync("user1", dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CancelReturnAsync_WhenRequested_ShouldCancelAndAddHistory()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Requested };

        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ReturnDetailsByUserSpecification>()))
            .ReturnsAsync(returnRequest);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CancelReturnAsync(TestGuid.FromInt(1), "user1");

        returnRequest.Status.Should().Be(ReturnStatus.Cancelled);
        returnRequest.ReturnHistories.Should().ContainSingle()
            .Which.Action.Should().Be("Cancelled");
        _unitOfWorkMock.Verify(u => u.GetRepository<ReturnRequest, Guid>().Update(returnRequest), Times.Once);
    }

    [Fact]
    public async Task CancelReturnAsync_WhenNotRequested_ShouldThrowBusinessRuleException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Approved };

        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ReturnDetailsByUserSpecification>()))
            .ReturnsAsync(returnRequest);

        var act = () => _returnService.CancelReturnAsync(TestGuid.FromInt(1), "user1");

        await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.BusinessRuleException>();
    }

    // ---------------- Admin ----------------

    [Fact]
    public async Task GetReturnByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        SetupGetReturn(null);

        var act = () => _returnService.GetReturnByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ApproveReturnAsync_WhenRequested_ShouldApproveAndAddHistory()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Requested };

        SetupGetReturn(returnRequest);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.ApproveReturnAsync(TestGuid.FromInt(1), "admin1");

        returnRequest.Status.Should().Be(ReturnStatus.Approved);
        returnRequest.ReturnHistories.Should().ContainSingle()
            .Which.PerformedById.Should().Be("admin1");
        _unitOfWorkMock.Verify(u => u.GetRepository<ReturnRequest, Guid>().Update(returnRequest), Times.Once);
    }

    [Fact]
    public async Task ApproveReturnAsync_WhenNotRequested_ShouldThrowBusinessRuleException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Completed };

        SetupGetReturn(returnRequest);

        var act = () => _returnService.ApproveReturnAsync(TestGuid.FromInt(1), "admin1");

        await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.BusinessRuleException>();
    }

    [Fact]
    public async Task RejectReturnAsync_WithoutReason_ShouldThrowArgumentException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Requested };

        SetupGetReturn(returnRequest);

        var act = () => _returnService.RejectReturnAsync(
            TestGuid.FromInt(1), "admin1", new RejectReturnRequestDTO { Reason = "" });

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task RejectReturnAsync_WithReason_ShouldRejectAndStoreReason()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Requested };

        SetupGetReturn(returnRequest);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.RejectReturnAsync(
            TestGuid.FromInt(1), "admin1", new RejectReturnRequestDTO { Reason = "Not eligible" });

        returnRequest.Status.Should().Be(ReturnStatus.Rejected);
        returnRequest.RejectionReason.Should().Be("Not eligible");
    }

    [Fact]
    public async Task ProcessRefundAsync_WhenProcessing_ShouldRefundWithMethod()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Processing };

        SetupGetReturn(returnRequest);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.ProcessRefundAsync(
            TestGuid.FromInt(1), "admin1", new ProcessRefundRequestDTO { RefundMethod = RefundMethod.OriginalPaymentMethod });

        returnRequest.Status.Should().Be(ReturnStatus.Refunded);
        returnRequest.RefundMethod.Should().Be(RefundMethod.OriginalPaymentMethod);
    }

    [Fact]
    public async Task MarkAsInTransitAsync_WhenNotApproved_ShouldThrowBusinessRuleException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Requested };

        SetupGetReturn(returnRequest);

        var act = () => _returnService.MarkAsInTransitAsync(TestGuid.FromInt(1), "admin1");

        await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.BusinessRuleException>();
    }

    [Fact]
    public async Task CompleteReturnAsync_WhenRefunded_ShouldComplete()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Refunded };

        SetupGetReturn(returnRequest);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CompleteReturnAsync(TestGuid.FromInt(1), "admin1");

        returnRequest.Status.Should().Be(ReturnStatus.Completed);
    }
    [Fact]
    public async Task ApproveItem_WithInvalidStatus_ThrowsTypedBusinessRuleWithoutSaving()
    {
        var item = new ReturnItem { Id = Guid.NewGuid(), Status = ReturnItemStatus.Rejected };
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnItem, Guid>().GetFirstOrDefaultAsync(It.IsAny<ReturnItemSpecification>())).ReturnsAsync(item);
        var act = () => _returnService.ApproveItemAsync(item.Id, "admin");
        await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.BusinessRuleException>();
        item.Status.Should().Be(ReturnItemStatus.Rejected);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
