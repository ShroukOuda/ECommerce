using ECommerce.Application.DTO.Review;

namespace ECommerce.Application.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<GetReviewDTO>> GetReviewsByProductIdAsync(int productId, CancellationToken ct = default);
    Task<GetReviewDTO> GetReviewByIdAsync(int id, CancellationToken ct = default);
    Task AddReviewAsync(AddReviewDTO dto, CancellationToken ct = default);
    Task DeleteReviewAsync(int id, CancellationToken ct = default);
}
