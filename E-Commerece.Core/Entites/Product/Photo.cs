namespace E_Commerece.Core.Entites.Product;

public class Photo:BaseEntity<int>
{
    public string ImageName { get; set; }
    public int ProductId { get; set; }
    
    //Navigation Properties
    // public virtual Product Product { get; set; }
}