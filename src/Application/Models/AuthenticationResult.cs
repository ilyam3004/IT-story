using Domain.Entities;

namespace Application.Services;

public class AuthenticationResult
{
    public string token { get; set; } = null!;
    public User user { get; set; } = null!;
}