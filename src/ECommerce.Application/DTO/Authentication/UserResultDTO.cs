namespace ECommerce.Application.DTO.Authentication;

public class UserResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}