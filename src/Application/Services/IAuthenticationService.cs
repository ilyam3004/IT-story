namespace Application.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult> Register(string nickname, string email, string password, string confirmPassword, string firstName, string lastName, string status);
    Task<AuthenticationResult> Login(string email, string passwrod); 
}
