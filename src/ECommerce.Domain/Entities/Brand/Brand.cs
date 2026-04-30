using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Brand;

namespace ECommerce.Domain.Entities.Brand;

public class Brand : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public BrandStatus Status { get; set; } = BrandStatus.Active;
    
    //Navigation Properties
    public virtual ICollection<BrandLogo> BrandLogos { get; set; } = new List<BrandLogo>();
    public virtual ICollection<Product.Product> Products { get; set; } = new List<Product.Product>();
}