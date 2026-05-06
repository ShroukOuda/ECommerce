using ECommerce.Application.Interfaces.Services;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace ECommerce.Infrastructure.Services;

public class TokenEncoder : ITokenEncoder
{
    public string EncodeToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        return WebEncoders.Base64UrlEncode(tokenBytes);
    }

    public string DecodeToken(string encodedToken)
    {
        var decodedBytes = WebEncoders.Base64UrlDecode(encodedToken);
        return Encoding.UTF8.GetString(decodedBytes);
    }
}