using Microsoft.AspNetCore.Mvc;
using Contracts.Authentication;
using Application.Services;
using ErrorOr;

namespace Api.Controllers;

[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        ErrorOr<AuthenticationResult> authResult = await _authenticationService.Register(
            request.username, 
            request.email, 
            request.password, 
            request.confirmPassword, 
            request.firstName, 
            request.lastName, 
            request.status);
        
        return authResult.Match(
            authenticationResult => Ok(MapAuthResult(authenticationResult)),
            errors => Problem(errors));
    }
    

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        ErrorOr<AuthenticationResult> authResult = await _authenticationService.Login(
            request.email, 
            request.password);
            
        return authResult.Match(
            authenticationResult => Ok(MapAuthResult(authenticationResult)),
            errors => Problem(errors));
    }

    private static AuthenticationResponse MapAuthResult(AuthenticationResult authResult)
        => new AuthenticationResponse
        {
            id = authResult.user.id,
            token = authResult.token,
            email = authResult.user.email,
            username = authResult.user.username,
            firstName = authResult.user.firstName,
            lastName = authResult.user.lastName,
            status = authResult.user.status
        };
}