using Microsoft.AspNetCore.Mvc;
using Contracts.Authentication;
using Application.Services;
using Domain.Entities;
using Application.Common.Interfaces.Persistence;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var authResult = await _authenticationService.Register(
            request.username, 
            request.email, 
            request.password, 
            request.confirmPassword, 
            request.firstName, 
            request.lastName, 
            request.status);
            
        return Ok();
    }
    

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var authResult = await _authenticationService.Login(
            request.email, 
            request.password);

        return Ok(new AuthenticationResponse
        {
            id = authResult.user.id,
            token = authResult.token,
            username = authResult.user.username,
            email = authResult.user.email,
            firstName = authResult.user.firstName,
            lastName = authResult.user.lastName,
            status = authResult.user.status
        });
    }
}