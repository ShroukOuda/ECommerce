
using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.CategoryImages;

public class UploadCategoryImageDTO : UploadImageDTO
{
    public int CategoryId { get; set; }
    public ImageSubType SubType { get; set; }
 
}