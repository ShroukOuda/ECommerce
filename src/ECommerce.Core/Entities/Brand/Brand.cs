using ECommerce.Core.Enums.Brand;

namespace ECommerce.Core.Entities.Brand;

public class Brand : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public BrandStatus Status { get; set; } = BrandStatus.Active;
    
    //Navigation Properties
    public virtual ICollection<BrandLogo> BrandLogos { get; set; } = new List<BrandLogo>();
    public virtual ICollection<Product.Product> Products { get; set; } = new List<Product.Product>();
}