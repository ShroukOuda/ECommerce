namespace ECommerce.Core.Entities;

public class Photo:BaseEntity<int>
{
    public string Url { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public PhotoType Type { get; set; }
    public PhotoSubType? SubType { get; set; }
    public string? EntityId { get; set; } 
    public int? ProductId { get; set; }    
    public int? CategoryId { get; set; }   

    // Navigation Properties
    public Product? Product { get; set; }
    public Category? Category { get; set; }
}