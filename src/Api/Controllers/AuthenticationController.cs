using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Contracts.Authentication;
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
            request.Username, 
            request.Email, 
            request.Password, 
            request.ConfirmPassword, 
            request.FirstName, 
            request.LastName, 
            request.Status);
        
        return authResult.Match(
            authenticationResult => Ok(MapAuthResult(authenticationResult)),
            errors => Problem(errors));
    }
    

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        ErrorOr<AuthenticationResult> authResult = await _authenticationService.Login(
            request.Email, 
            request.Password);
            
        return authResult.Match(
            authenticationResult => Ok(MapAuthResult(authenticationResult)),
            errors => Problem(errors));
    }

    private static AuthenticationResponse MapAuthResult(AuthenticationResult authResult)
        => new AuthenticationResponse
        {
            Id = authResult.User.Id,
            Token = authResult.Token,
            Email = authResult.User.Email,
            Username = authResult.User.Username,
            FirstName = authResult.User.FirstName,
            LastName = authResult.User.LastName,
            Status = authResult.User.Status
        };
}