using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.CategoryImages;

public class CategoryImageDTO : BaseImageDTO
{
    public ImageSubType SubType { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
}