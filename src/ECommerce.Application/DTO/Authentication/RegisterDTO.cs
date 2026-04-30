using ECommerce.Domain.Enums.User;

namespace ECommerce.Application.DTO.Authentication;

public class RegisterDTO
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string CountryCode { get; set; } = null;
    public string PhoneNumber { get; set; }  = null!; 
    public Gender Gender { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
}