using System.Net.Mail;
using ErrorOr;
using Domain.Entities;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Persistence;
using Domain.Common.Errors;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private readonly IUserRepository _userRepository;
    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<AuthenticationResult>> Register(string username, string email, string password, string confirmPassword, string firstName, string lastName, string status)
    {
        if (password.Length is > 60 or < 8)
            return Errors.Authentication.InvalidPassword;
        
        if (await _userRepository.GetByEmail(email) is not null)
            return Errors.Authentication.DuplicateEmail;

        if (!MailAddress.TryCreate(email, out var outEmail))
            return Errors.Authentication.InvalidEmail;

        if (password != confirmPassword)
            return Errors.Authentication.PasswordsDoNotMatch;

        var user = new User
        {
            username = username, 
            email = email, 
            password = BCrypt.Net.BCrypt.HashPassword(password), 
            firstName = firstName, 
            lastName = lastName, 
            status = status
        };

        await _userRepository.Add(user);

        var tokenUser = await _userRepository.GetByEmail(user.email);

        return new AuthenticationResult
        { 
            user = user,
            token = _jwtTokenGenerator.GenerateToken(tokenUser)
        };
    }

    public async Task<ErrorOr<AuthenticationResult>> Login(string email, string password)
    {

        var user = await _userRepository.GetByEmail(email);
        
        if (user is null)
            return Errors.Authentication.UserNotFound;
        
        if (!BCrypt.Net.BCrypt.Verify(password, user.password))
            return Errors.Authentication.UserNotFound;

        return new AuthenticationResult
        {
            user = user,
            token = _jwtTokenGenerator.GenerateToken(user)
        };
    }
}