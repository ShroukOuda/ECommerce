using ECommerce.Application.DTO.Review;

namespace ECommerce.Application.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<GetReviewDTO>> GetReviewsByProductIdAsync(Guid productId);
    Task<GetReviewDTO> GetReviewByIdAsync(Guid id);
    Task<GetReviewDTO> AddReviewAsync(string userId, AddReviewDTO dto);
    Task DeleteReviewAsync(string userId, Guid id);
}
