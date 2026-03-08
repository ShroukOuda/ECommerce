namespace ECommerce.Core.Entities.User;

public class UserSession : BaseEntity<int>
{
    public string SessionToken { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    //FK
    public string UserId { get; set; }
    
    //Navigation Properties
    public virtual User? User { get; set; }
}