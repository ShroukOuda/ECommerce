namespace ECommerce.Core.Entities.Product;

public class ProductOptionValue : BaseEntity<int>
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal PriceValue { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    
    public int OptionId { get; set; } //FK
    
    //Navigation Properties
    public virtual ProductOption? ProductOption { get; set; }
    public virtual ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();

}