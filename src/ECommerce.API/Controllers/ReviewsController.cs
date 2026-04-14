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

    [HttpGet("get-by-product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
        return Ok(reviews);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        return Ok(review);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddReviewDTO dto)
    {
        await _reviewService.AddReviewAsync(dto);
        return Ok(new ResponseAPI(200, "Review added successfully"));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return Ok(new ResponseAPI(200, "Review deleted successfully"));
    }
}
