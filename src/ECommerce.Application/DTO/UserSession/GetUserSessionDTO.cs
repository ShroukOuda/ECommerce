namespace ECommerce.Application.DTO.UserSession;

public class GetUserSessionDTO
{
    public int Id { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
