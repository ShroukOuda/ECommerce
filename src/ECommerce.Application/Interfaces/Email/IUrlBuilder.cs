namespace ECommerce.Application.Interfaces.Email;

public interface IUrlBuilder
{
    string EmailConfirmation(string userId, string rawToken);
 
    string PasswordReset(string userId, string rawToken);
 
    string OrderDetails(string orderId);
 
    string RevokeAllSessions();
 
    string ProductList();
}