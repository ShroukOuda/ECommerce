
using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.CategoryImages;

public class UploadCategoryImageDTO : UploadImageDTO
{
    public ImageSubType SubType { get; set; }
 
}