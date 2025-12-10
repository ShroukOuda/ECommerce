namespace E_Commerece.Core.Entities;

public class Photo:BaseEntity<int>
{
    public string ImageName { get; set; }
    public int ProductId { get; set; }
    
    //Navigation Properties
    // public virtual Product Product { get; set; }
}