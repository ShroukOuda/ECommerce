

namespace ECommerce.Domain.Entities.Users;

public class UserSession : BaseEntity<Guid>
{
    public string RefreshToken { get; set; } = string.Empty;

    public string IpAddress { get; set; }  = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool  IsActive { get; set; } = true;

    public bool IsRevoked => !IsActive;
    public bool IsExpired => DateTime.UtcNow >= RefreshTokenExpiresAt;
    public bool IsValid => IsActive && !IsExpired;

    // FK
    public string UserId { get; set; } = string.Empty;

    // Navigation property
    public  User User { get; set; } = null!;
}