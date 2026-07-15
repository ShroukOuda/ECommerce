
using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.CategoryImages;

public class UploadBrandLogoDTO : UploadImageDTO
{
    public Guid BrandId { get; set; }
    public ImageSubType SubType { get; set; }
 
}