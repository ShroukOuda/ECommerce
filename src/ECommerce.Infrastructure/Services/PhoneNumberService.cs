using ECommerce.Application.Interfaces.Services;
using PhoneNumbers;

namespace ECommerce.Infrastructure.Services;

public class PhoneNumberService : IPhoneNumberService
{
    private readonly PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();

    public bool IsValid(string phoneNumber, string countryCode)
    {
        try
        {
            var parsedNumber = _phoneNumberUtil.Parse(phoneNumber, countryCode);
            return _phoneNumberUtil.IsValidNumber(parsedNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
}