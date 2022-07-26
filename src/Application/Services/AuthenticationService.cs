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
        if (_userRepository.GetByEmail(email) is not null)
            return Errors.User.DuplicateEmail;
        
        var user = new User
        {
            username = username, 
            email = email, 
            password = password, 
            firstName = firstName, 
            lastName = lastName, 
            status = status
        };

        await _userRepository.Add(user);

        return new AuthenticationResult
        { 
            user = user,
            token = _jwtTokenGenerator.GenerateToken(user)
        };
    }

    public async Task<ErrorOr<AuthenticationResult>> Login(string email, string password)
    {

        var user = await _userRepository.GetByEmail(email);

        if (user is null)
            return Errors.Authentication.InvallidCredentials;

        if (user.password != password)
            return Errors.Authentication.InvallidCredentials;

        return new AuthenticationResult
        {
            user = user,
            token = _jwtTokenGenerator.GenerateToken(user)
        };
    }
}