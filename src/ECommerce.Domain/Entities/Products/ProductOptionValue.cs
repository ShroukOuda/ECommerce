using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Products;

public class ProductOptionValue : BaseEntity<Guid>
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal PriceValue { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    
    public Guid OptionId { get; set; } //FK
    
    //Navigation Properties
    public virtual ProductOption? ProductOption { get; set; }
    public virtual ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();

}