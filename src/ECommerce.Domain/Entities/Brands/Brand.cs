using ECommerce.Domain.Enums.Brand;

namespace ECommerce.Domain.Entities.Brands;

public class Brand : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public BrandStatus Status { get; set; } = BrandStatus.Active;
    
    //Navigation Properties
    public  ICollection<BrandLogo> BrandLogos { get; set; } = new List<BrandLogo>();
    public  ICollection<Product> Products { get; set; } = new List<Product>();
}