using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.CategoryImages;

public class BrandLogoDTO : BaseImageDTO
{
    public ImageSubType SubType { get; set; }
    public Guid BrandId { get; set; }
    public string BrandName { get; set; } = null!;
}