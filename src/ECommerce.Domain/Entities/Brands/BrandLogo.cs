using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Brands;

public class BrandLogo : BaseImage
{
    public Guid BrandId { get; set; }
    
    //Navigation Properties
    public virtual  Brand? Brand { get; set; }
}