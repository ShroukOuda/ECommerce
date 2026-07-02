namespace ECommerce.Application.DTO.Auth;

public class ResetPasswordDTO
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}