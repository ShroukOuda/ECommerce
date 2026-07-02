namespace ECommerce.Application.Interfaces.Email;

public interface IUrlBuilder
{
    string EmailConfirmation(string email, string rawToken);
 
    string PasswordReset(string email, string rawToken);
 
    string OrderDetails(string orderId);
 
    string RevokeAllSessions();
 
    string ProductList();
}