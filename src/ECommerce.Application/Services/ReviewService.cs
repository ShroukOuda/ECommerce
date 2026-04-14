using ECommerce.Application.DTO.Review;
using ECommerce.Core.Entities.Review;
using ECommerce.Core.Interfaces.Repositories;

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
        var reviews = await _unitOfWork.ProductReviewRepository.GetReviewsByProductIdAsync(productId, ct);
        return _mapper.Map<IEnumerable<GetReviewDTO>>(reviews);
    }

    public async Task<GetReviewDTO> GetReviewByIdAsync(Guid id, CancellationToken ct = default)
    {
        var review = await _unitOfWork.ProductReviewRepository.GetByIdAsync(id, ct);
        if (review is null) throw new KeyNotFoundException($"Review with ID {id} not found.");
        return _mapper.Map<GetReviewDTO>(review);
    }

    public async Task AddReviewAsync(AddReviewDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var review = _mapper.Map<ProductReview>(dto);
        await _unitOfWork.ProductReviewRepository.AddAsync(review, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteReviewAsync(Guid id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.ProductReviewRepository.ExistsAsync(r => r.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Review with ID {id} not found.");
        var stub = new ProductReview { Id = id };
        await _unitOfWork.ProductReviewRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
