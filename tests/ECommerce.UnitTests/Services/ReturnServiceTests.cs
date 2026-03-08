using AutoMapper;
using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Return;
using ECommerce.Core.Interfaces.Repositories;
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

    [Fact]
    public async Task GetReturnsByUserIdAsync_ShouldReturnMappedReturns()
    {
        var returns = new List<ReturnRequest> { new() { Id = 1, UserId = "user1" } };
        var returnDtos = new List<GetReturnRequestDTO> { new() { Id = 1, UserId = "user1" } };

        _unitOfWorkMock.Setup(u => u.ReturnRequestRepository.GetReturnsByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetReturnRequestDTO>>(returns)).Returns(returnDtos);

        var result = await _returnService.GetReturnsByUserIdAsync("user1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetReturnByIdAsync_WhenExists_ShouldReturnRequest()
    {
        var returnReq = new ReturnRequest { Id = 1, ReturnNumber = "RET-001" };
        var returnDto = new GetReturnRequestDTO { Id = 1, ReturnNumber = "RET-001" };

        _unitOfWorkMock.Setup(u => u.ReturnRequestRepository.GetReturnWithItemsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnReq);
        _mapperMock.Setup(m => m.Map<GetReturnRequestDTO>(returnReq)).Returns(returnDto);

        var result = await _returnService.GetReturnByIdAsync(1);

        result.Should().NotBeNull();
        result.ReturnNumber.Should().Be("RET-001");
    }

    [Fact]
    public async Task GetReturnByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ReturnRequestRepository.GetReturnWithItemsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReturnRequest?)null);

        var act = () => _returnService.GetReturnByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateReturnRequestAsync_WithValidData_ShouldCreateReturn()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = 1,
            UserId = "user1",
            Reason = "Defective",
            Items = new List<CreateReturnItemDTO> { new() }
        };
        var returnDto = new GetReturnRequestDTO { Id = 1 };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.ReturnRequestRepository.AddAsync(It.IsAny<ReturnRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetReturnRequestDTO>(It.IsAny<ReturnRequest>())).Returns(returnDto);

        var result = await _returnService.CreateReturnRequestAsync(dto);

        result.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.ReturnRequestRepository.AddAsync(It.IsAny<ReturnRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateReturnRequestAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateReturnRequestDTO { OrderId = 0 };
        var failures = new List<ValidationFailure> { new("OrderId", "OrderId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _returnService.CreateReturnRequestAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
