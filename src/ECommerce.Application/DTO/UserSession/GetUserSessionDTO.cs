namespace ECommerce.Application.DTO.UserSession;

public class GetUserSessionDTO
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public bool IsExpired { get; set; }

}
