namespace ECommerce.Core.Entities.Brand;

public class BrandLogo : BaseImage
{
    public int BrandId { get; set; }
    
    //Navigation Properties
    public virtual  Brand? Brand { get; set; }
}