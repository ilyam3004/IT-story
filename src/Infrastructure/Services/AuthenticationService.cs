using System.Net.Mail;
using Application.Common.Interfaces.Authentication;
using ErrorOr;
using Domain.Entities;
using Application.Common.Interfaces.Persistence;
using Application.Models;
using Application.Services;
using Domain.Common.Errors;

namespace Infrastructure.Services;

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
            Username = username, 
            Email = email, 
            Password = BCrypt.Net.BCrypt.HashPassword(password), 
            FirstName = firstName, 
            LastName = lastName, 
            Status = status
        };

        await _userRepository.Add(user);

        var tokenUser = await _userRepository.GetByEmail(user.Email);

        return new AuthenticationResult
        { 
            User = user,
            Token = _jwtTokenGenerator.GenerateToken(tokenUser!)
        };
    }

    public async Task<ErrorOr<AuthenticationResult>> Login(string email, string password)
    {

        var user = await _userRepository.GetByEmail(email);
        
        if (user is null)
            return Errors.Authentication.UserNotFound;
        
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            return Errors.Authentication.UserNotFound;

        return new AuthenticationResult
        {
            User = user,
            Token = _jwtTokenGenerator.GenerateToken(user)
        };
    }
}