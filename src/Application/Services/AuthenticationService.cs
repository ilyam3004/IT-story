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
        //1. Validate the user doesn't exist
        if(_userRepository.GetByEmail(email) is not null)
            throw new Exception("User already exists");
        

        //2. Create user (Generate ID)
        var user = new User()
        {
            firstName = firstName,
            lastName = lastName,
            email = email,
            status = status,
            username = username,
            password = password
        };

        _userRepository.Add(user);

        //3. Generate token
        
        //TODO replace 1 with user id from database
        var token = _jwtTokenGenerator.GenerateToken(user);
        
        return new AuthenticationResult
        {
            user = user,
            token = token
        };
    }

    public async Task<AuthenticationResult> Login(string email, string password)
    {

        if (_userRepository.GetByEmail(email) is not User user)
            throw new Exception("User with this email does not exist");
        
        if (user.password != password)
            throw new Exception("Incorrect password");

        var token = _jwtTokenGenerator.GenerateToken(user);
        
        return new AuthenticationResult
        {
            user = user,
            token = token
        };
    }
}