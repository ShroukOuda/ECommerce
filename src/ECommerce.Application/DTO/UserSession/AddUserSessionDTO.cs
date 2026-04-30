namespace ECommerce.Application.DTO.UserSession;

public class AddUserSessionDTO
{
    public string SessionToken { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string UserId { get; set; } = string.Empty;
}