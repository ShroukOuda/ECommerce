using ECommerce.Application.DTO.Review;
using ECommerce.Domain.Entities.Reviews;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Reviews;

namespace ECommerce.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddReviewDTO> _addValidator;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AddReviewDTO> addValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
    }

    public async Task<IEnumerable<GetReviewDTO>> GetReviewsByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var spec = new ReviewByProductSpecification(productId);
        var reviews = await _unitOfWork.GetRepository<ProductReview, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetReviewDTO>>(reviews);
    }

    public async Task<GetReviewDTO> GetReviewByIdAsync(Guid id, CancellationToken ct = default)
    {
        var review = await _unitOfWork.GetRepository<ProductReview, Guid>().GetByIdAsync(id, ct);
        if (review is null) throw new KeyNotFoundException($"Review with ID {id} not found.");
        return _mapper.Map<GetReviewDTO>(review);
    }

    public async Task<GetReviewDTO> AddReviewAsync(string userId, AddReviewDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var review = _mapper.Map<ProductReview>(dto);
        review.UserId = userId;
        await _unitOfWork.GetRepository<ProductReview, Guid>().AddAsync(review, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetReviewDTO>(review);
    }

    public async Task DeleteReviewAsync(string userId, Guid id, CancellationToken ct = default)
    {
        var spec = new ReviewByUserSpecification(userId, id);
        bool exists = await _unitOfWork.GetRepository<ProductReview, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Review with ID {id} not found.");
        var stub = new ProductReview { Id = id };
        _unitOfWork.GetRepository<ProductReview, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
