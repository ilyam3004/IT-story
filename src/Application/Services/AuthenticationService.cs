using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Persistence;
using Domain.Entities;

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

    public async Task<AuthenticationResult> Register(string username, string email, string password, string confirmPassword, string firstName, string lastName, string status)
    {   
        
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

    public async Task<AuthenticationResult> Login(string email, string password)
    {

        var user = await _userRepository.GetByEmail(email);
        
        return new AuthenticationResult
        {
            user = user,
            token = "toked"
        };
    }
}