namespace ECommerce.Application.Interfaces.Services;

public interface ITokenEncoder
{
    string EncodeToken(string token);
    string DecodeToken(string encodedToken);
}