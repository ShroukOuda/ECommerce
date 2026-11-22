using AutoMapper;
using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Returns;
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
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
        };
        var returnRequest = new ReturnRequest { OrderId = dto.OrderId };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<ReturnRequest>(dto)).Returns(returnRequest);
        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _returnService.CreateReturnAsync("user1", dto);

        returnRequest.UserId.Should().Be("user1");
        returnRequest.Status.Should().Be(ReturnStatus.Requested);
        returnRequest.ReturnNumber.Should().NotBeNullOrEmpty();
        returnRequest.ReturnHistories.Should().ContainSingle()
            .Which.NewStatus.Should().Be(ReturnStatus.Requested);
        _unitOfWorkMock.Verify(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(returnRequest), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
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
    public async Task CancelReturnAsync_WhenNotRequested_ShouldThrowInvalidOperationException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Approved };

        _unitOfWorkMock.Setup(u => u.GetRepository<ReturnRequest, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ReturnDetailsByUserSpecification>()))
            .ReturnsAsync(returnRequest);

        var act = () => _returnService.CancelReturnAsync(TestGuid.FromInt(1), "user1");

        await act.Should().ThrowAsync<InvalidOperationException>();
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
    public async Task ApproveReturnAsync_WhenNotRequested_ShouldThrowInvalidOperationException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Completed };

        SetupGetReturn(returnRequest);

        var act = () => _returnService.ApproveReturnAsync(TestGuid.FromInt(1), "admin1");

        await act.Should().ThrowAsync<InvalidOperationException>();
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
    public async Task MarkAsInTransitAsync_WhenNotApproved_ShouldThrowInvalidOperationException()
    {
        var returnRequest = new ReturnRequest { Id = TestGuid.FromInt(1), Status = ReturnStatus.Requested };

        SetupGetReturn(returnRequest);

        var act = () => _returnService.MarkAsInTransitAsync(TestGuid.FromInt(1), "admin1");

        await act.Should().ThrowAsync<InvalidOperationException>();
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
}
