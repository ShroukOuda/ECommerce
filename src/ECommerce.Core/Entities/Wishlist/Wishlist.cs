namespace ECommerce.Core.Entities.Wishlist;

public class Wishlist : BaseEntity<int>
{
    
    //FK
    public int ProductId { get; set; }
    
    //Navigation Properties
    public virtual Product.Product? Product { get; set; }
}