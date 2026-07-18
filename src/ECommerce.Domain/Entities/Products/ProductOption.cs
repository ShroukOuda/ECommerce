using ECommerce.Domain.Enums;
using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Product;

namespace ECommerce.Domain.Entities.Products;

public class ProductOption : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public OptionDisplayType DisplayType { get; set; } = OptionDisplayType.Dropdown;
    public OptionType Type { get; set; } = OptionType.VariantSelector;
    public string AttributeKey { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public decimal PriceValue { get; set; }
    public int SortOrder { get; set; }
    
    public Guid ProductId { get; set; } // FK
    
    //Navigation Properties
    public  Product Product { get; set; } = null!;
    public  ICollection<ProductOptionValue> ProductOptionValues { get; set; } = new List<ProductOptionValue>();
    public  ICollection<CartItemOption> CartItemOptions { get; set; } = new List<CartItemOption>();
}