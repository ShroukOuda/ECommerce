using ECommerce.Domain.Enums.User;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Auth;

public class UpdateUserProfileDTO
{
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public DateTime? DateOfBirth { get; set; }
    public string? CountryCode { get; set; } 
    public string? PhoneNumber { get; set; } 
    public Gender? Gender { get; set; }
    public IFormFile? ProfilePictureUrl { get; set; }
}