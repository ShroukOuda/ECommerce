namespace ECommerce.Application.Interfaces.Services;

public interface IPhoneNumberService
{ 
    bool IsValid(string phoneNumber, string countryCode);
}