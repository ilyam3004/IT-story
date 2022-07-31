using Domain.Entities;

namespace Application.Models;

public class AuthenticationResult
{
    public string Token { get; set; } = null!;
    public User User { get; set; } = null!;
}