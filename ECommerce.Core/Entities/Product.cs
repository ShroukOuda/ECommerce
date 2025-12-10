namespace E_Commerece.Core.Entities;

public class Product:BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public int CategoryId { get; set; }
    
    //Navigation Properties
    public virtual Category Category { get; set; }
    public virtual List<Photo> Photos { get; set; }
}