using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities.Product;

public class ProductOption : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public OptionDisplayType DisplayType { get; set; } = OptionDisplayType.Dropdown;
    public OptionType Type { get; set; } = OptionType.VariantSelector;
    public string AttributeKey { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public decimal PriceValue { get; set; }
    public int SortOrder { get; set; }
    
    public int ProductId { get; set; } // FK
    
    //Navigation Properties
    public virtual Product? Product { get; set; }
    public virtual ICollection<ProductOptionValue> ProductOptionValues { get; set; } = new List<ProductOptionValue>();
    public virtual ICollection<Cart.CartItemOption> CartItemOptions { get; set; } = new List<Cart.CartItemOption>();
}