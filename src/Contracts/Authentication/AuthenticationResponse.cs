namespace Contracts.Authentication;

public class AuthenticationResponse
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}