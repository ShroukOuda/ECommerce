using AutoMapper;
using ECommerce.Application.DTO.Review;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Review;
using ECommerce.Core.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<AddReviewDTO>> _addValidatorMock;
    private readonly IReviewService _reviewService;

    public ReviewServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _addValidatorMock = new Mock<IValidator<AddReviewDTO>>();
        _reviewService = new ReviewService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _addValidatorMock.Object);
    }

    [Fact]
    public async Task GetReviewsByProductIdAsync_ShouldReturnMappedReviews()
    {
        var reviews = new List<ProductReview> { new() { Id = 1, Rating = 5 } };
        var reviewDtos = new List<GetReviewDTO> { new() { Id = 1, Rating = 5 } };

        _unitOfWorkMock.Setup(u => u.ProductReviewRepository.GetReviewsByProductIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetReviewDTO>>(reviews)).Returns(reviewDtos);

        var result = await _reviewService.GetReviewsByProductIdAsync(1);

        result.Should().HaveCount(1);
        result.First().Rating.Should().Be(5);
    }

    [Fact]
    public async Task GetReviewByIdAsync_WhenExists_ShouldReturnReview()
    {
        var review = new ProductReview { Id = 1, Rating = 5 };
        var reviewDto = new GetReviewDTO { Id = 1, Rating = 5 };

        _unitOfWorkMock.Setup(u => u.ProductReviewRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<GetReviewDTO>(review)).Returns(reviewDto);

        var result = await _reviewService.GetReviewByIdAsync(1);

        result.Should().NotBeNull();
        result.Rating.Should().Be(5);
    }

    [Fact]
    public async Task GetReviewByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductReviewRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductReview?)null);

        var act = () => _reviewService.GetReviewByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddReviewAsync_WithValidData_ShouldAddReview()
    {
        var dto = new AddReviewDTO { Rating = 5, Title = "Great!", ProductId = 1, OrderId = 1, UserId = "user1" };
        var review = new ProductReview { Rating = 5 };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<ProductReview>(dto)).Returns(review);
        _unitOfWorkMock.Setup(u => u.ProductReviewRepository.AddAsync(It.IsAny<ProductReview>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _reviewService.AddReviewAsync(dto);

        _unitOfWorkMock.Verify(u => u.ProductReviewRepository.AddAsync(review, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddReviewAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new AddReviewDTO { Rating = 0 };
        var failures = new List<ValidationFailure> { new("Rating", "Rating must be between 1 and 5") };

        _addValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _reviewService.AddReviewAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteReviewAsync_WhenExists_ShouldDeleteReview()
    {
        _unitOfWorkMock.Setup(u => u.ProductReviewRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductReview, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _reviewService.DeleteReviewAsync(1);

        _unitOfWorkMock.Verify(u => u.ProductReviewRepository.DeleteAsync(It.IsAny<ProductReview>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteReviewAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.ProductReviewRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<ProductReview, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _reviewService.DeleteReviewAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
