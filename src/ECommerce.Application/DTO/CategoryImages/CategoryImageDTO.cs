using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.CategoryImages;

public class CategoryImageDTO : BaseImageDTO
{
    public ImageSubType SubType { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
}