using ECommerce.Application.DTO.Review;

namespace ECommerce.Application.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<GetReviewDTO>> GetReviewsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<GetReviewDTO> GetReviewByIdAsync(Guid id, CancellationToken ct = default);
    Task AddReviewAsync(AddReviewDTO dto, CancellationToken ct = default);
    Task DeleteReviewAsync(Guid id, CancellationToken ct = default);
}
