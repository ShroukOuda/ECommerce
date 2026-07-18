using ECommerce.Domain.Enums.Media;

namespace ECommerce.Domain.Entities.Brands;

public class BrandLogo : BaseImage
{
    public Guid BrandId { get; set; }
    public ImageSubType SubType { get; set; }
    
    //Navigation Properties
    public Brand Brand { get; set; } = null!;
}