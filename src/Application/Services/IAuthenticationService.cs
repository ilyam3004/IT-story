using ErrorOr;

namespace Application.Services;

public interface IAuthenticationService
{
    Task<ErrorOr<AuthenticationResult>> Register(string username, string email, string password, string confirmPassword, string firstName, string lastName, string status);
    Task<ErrorOr<AuthenticationResult>> Login(string email, string password); 
}
