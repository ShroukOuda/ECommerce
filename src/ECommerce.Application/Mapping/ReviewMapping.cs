using ECommerce.Application.DTO.Review;
using ECommerce.Domain.Entities.Reviews;

namespace ECommerce.Application.Mapping;

public class ReviewMapping : Profile
{
    public ReviewMapping()
    {
        CreateMap<AddReviewDTO, ProductReview>();
        CreateMap<ProductReview, GetReviewDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
