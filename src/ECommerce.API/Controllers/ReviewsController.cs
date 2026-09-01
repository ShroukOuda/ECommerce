using ECommerce.Application.DTO.Review;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class ReviewsController : BaseController
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private string currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
        return Success(
            reviews,
            "Reviews retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        return Success(
            review,
            "Review retrieved successfully.");
    }

    [HttpPost()]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Add(AddReviewDTO dto)
    {
        var review = await _reviewService.AddReviewAsync(currentUserId, dto);
        return Created(
            review,
            "Review added successfully.");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _reviewService.DeleteReviewAsync(currentUserId, id);
        return NoContent();
    }
}
